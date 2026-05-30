using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Email;
using LIMTIC.Application.Abstractions.Profiles;
using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.Abstractions.Storage;
using LIMTIC.Application.Abstractions.UserManagement;
using LIMTIC.Application.Emails.Models;
using LIMTIC.Application.IOC;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Abstractions.Contact;
using LIMTIC.Domain.Abstractions.Events;
using LIMTIC.Domain.Abstractions.Publications;
using LIMTIC.Domain.Abstractions.ResearchAxis;
using LIMTIC.Domain.Abstractions.Users;
using LIMTIC.Infrastructure.Data;
using LIMTIC.Infrastructure.IOC;
using LIMTIC.UnitTests.Helpers;
using LIMTIC.UnitTests.Mocks;
using LIMTIC.WebAPI.IOC;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LIMTIC.UnitTests.Base
{
    public abstract class BaseTests : IDisposable
    {
        protected readonly ServiceProvider ServiceProvider;
        protected readonly AppDbContext DbContext;

        // ── User / auth ────────────────────────────────────────────────────────
        protected IUserRepository UserRepository
            => ServiceProvider.GetRequiredService<IUserRepository>();
        protected IUsersManagementService UsersManagementService
            => ServiceProvider.GetRequiredService<IUsersManagementService>();
        protected IMasterianRepository MasterianRepository
            => ServiceProvider.GetRequiredService<IMasterianRepository>();
        protected IResearcherRepository ResearcherRepository
            => ServiceProvider.GetRequiredService<IResearcherRepository>();
        protected IPhDStudentRepository PhDStudentRepository
            => ServiceProvider.GetRequiredService<IPhDStudentRepository>();
        protected IResetPasswordRepository ResetPasswordRepository
            => ServiceProvider.GetRequiredService<IResetPasswordRepository>();
        protected IRefreshTokenRepository RefreshTokenRepository
            => ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

        // ── Profiles ───────────────────────────────────────────────────────────
        protected IResearcherProfileService ResearcherProfileService
            => ServiceProvider.GetRequiredService<IResearcherProfileService>();
        protected IPhDStudentProfileService PhDStudentProfileService
            => ServiceProvider.GetRequiredService<IPhDStudentProfileService>();
        protected IMasterianProfileService MasterianProfileService
            => ServiceProvider.GetRequiredService<IMasterianProfileService>();

        // ── Research axis ──────────────────────────────────────────────────────
        protected IResearchAxisService ResearchAxisService
            => ServiceProvider.GetRequiredService<IResearchAxisService>();
        protected IResearchAxisRepository ResearchAxisRepository
            => ServiceProvider.GetRequiredService<IResearchAxisRepository>();

        // ── Events / contact ───────────────────────────────────────────────────
        protected IEventsRepository EventsRepository
            => ServiceProvider.GetRequiredService<IEventsRepository>();
        protected IContactRepository ContactRepository
            => ServiceProvider.GetRequiredService<IContactRepository>();

        // ── Publications ───────────────────────────────────────────────────────
        protected IPublicationRepository PublicationRepository
            => ServiceProvider.GetRequiredService<IPublicationRepository>();
        protected IJournalArticleRepository JournalArticleRepository
            => ServiceProvider.GetRequiredService<IJournalArticleRepository>();
        protected ITechnicalReportRepository TechnicalReportRepository
            => ServiceProvider.GetRequiredService<ITechnicalReportRepository>();
        protected IBookChapterRepository BookChapterRepository
            => ServiceProvider.GetRequiredService<IBookChapterRepository>();
        protected INationalConferenceRepository NationalConferenceRepository
            => ServiceProvider.GetRequiredService<INationalConferenceRepository>();
        protected IInternationalConferenceRepository InternationalConferenceRepository
            => ServiceProvider.GetRequiredService<IInternationalConferenceRepository>();
        protected IPublicationService PublicationService
            => ServiceProvider.GetRequiredService<IPublicationService>();

        // ── Infrastructure ─────────────────────────────────────────────────────
        protected IUnitOfWork UnitOfWork
            => ServiceProvider.GetRequiredService<IUnitOfWork>();

        /// <summary>
        /// Returns the <see cref="TestCurrentUserService"/> so individual tests can
        /// adjust <see cref="TestCurrentUserService.UserId"/> and
        /// <see cref="TestCurrentUserService.Role"/> without needing a Mock.
        /// </summary>
        protected TestCurrentUserService CurrentUserService
            => (TestCurrentUserService)ServiceProvider.GetRequiredService<ICurrentUserService>();

        protected BaseTests()
        {
            var services = new ServiceCollection();

            var configurationData = new Dictionary<string, string>
            {
                { "Jwt:Key",                            "TestJwtSecretKeyForUnitTestsWithMinimumLength32Characters" },
                { "Jwt:Issuer",                         "TestIssuer" },
                { "Jwt:Audience",                       "TestAudience" },
                { "Jwt:ExpiryMinutes",                  "60" },
                { "RefreshToken:ExpiryDays",            "7" },
                { "OTPToken:ExpiryMinutes",             "10" },
                { "ResetPasswordToken:ExpiryMinutes",   "30" },
                { "Email:SmtpServer",                   "localhost" },
                { "Email:SmtpPort",                     "587" },
                { "Email:SenderEmail",                  "test@example.com" },
                { "Email:SenderPassword",               "password" },
                { "Email:EnableSSL",                    "false" },
                { "BlobStorage:ConnectionStringSecretName", "BlobStorageConnectionString" },
                { "KeyvaultUri",                        "https://test.vault.azure.net/" },
                { "DefaultConnection",                  "Server=localhost;Database=limtic_test;User Id=test;Password=test;" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            services.AddInfrastructure(configuration);
            services.AddApplication();
            services.AddMappers();
            services.AddWebApi();

            var dpKeysPath = Path.Combine(Directory.GetCurrentDirectory(), "DataProtection-Keys");
            Directory.CreateDirectory(dpKeysPath);
            services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(dpKeysPath));

            // Replace real email with a no-op fake
            var emailDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IEmailService));
            if (emailDescriptor != null) services.Remove(emailDescriptor);
            services.AddScoped<IEmailService, FakeEmailService>();

            // Replace real blob storage with a lightweight in-memory stub
            var blobDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IBlobStorageService));
            if (blobDescriptor != null) services.Remove(blobDescriptor);
            services.AddSingleton<IBlobStorageService, MockBlobStorageService>();

            // Replace ICurrentUserService with a mutable test stub.
            // Tests cast it back to TestCurrentUserService to configure UserId / Role.
            var currentUserDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ICurrentUserService));
            if (currentUserDescriptor != null) services.Remove(currentUserDescriptor);
            services.AddScoped<ICurrentUserService, TestCurrentUserService>();

            // Use a fresh in-memory sqlite DB per test class instance
            var dbDescriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (dbDescriptor != null)
                services.Remove(dbDescriptor);

            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open(); 

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connection),
                ServiceLifetime.Scoped);

            ServiceProvider = services.BuildServiceProvider();
            DbContext = ServiceProvider.GetRequiredService<AppDbContext>();
            DbContext.Database.EnsureCreated();
        }

        // ── Private helpers ────────────────────────────────────────────────────

        private sealed class FakeEmailService : IEmailService
        {
            public Task SendOTPEmailAsync(OTPEmailModel model) => Task.CompletedTask;
            public Task SendContactEmailAsync(ContactEmailModel model) => Task.CompletedTask;
            public Task<bool> TestSmtpAsync(string testEmail) => Task.FromResult(true);
        }

        public void Dispose()
        {
            DbContext?.Dispose();
            if (ServiceProvider is IDisposable disposable)
                disposable.Dispose();
        }
    }
}