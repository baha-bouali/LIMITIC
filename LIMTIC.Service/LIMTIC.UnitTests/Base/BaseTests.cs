using LIMTIC.Application.Abstractions.Email;
using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Profiles;
using LIMTIC.Application.Abstractions.UserManagement;
using LIMTIC.Application.Abstractions.Storage;
using LIMTIC.Application.Emails.Models;
using LIMTIC.Application.IOC;
using LIMTIC.Infrastructure.Data;
using LIMTIC.Infrastructure.IOC;
using LIMTIC.UnitTests.Helpers;
using LIMTIC.WebAPI.IOC;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using LIMTIC.Domain.Abstractions.Events;
using LIMTIC.Domain.Abstractions.Users;
using LIMTIC.Domain.Abstractions.ResearchAxis;

namespace LIMTIC.UnitTests.Base
{
    public abstract class BaseTests : IDisposable
    {
        protected readonly ServiceProvider ServiceProvider;
        protected readonly AppDbContext DbContext;
        protected IUserRepository UserRepository => ServiceProvider.GetRequiredService<IUserRepository>();
        protected IUsersManagementService UsersManagementService => ServiceProvider.GetRequiredService<IUsersManagementService>();
        protected IMasterianRepository MasterianRepository => ServiceProvider.GetRequiredService<IMasterianRepository>();
        protected IResearcherRepository ResearcherRepository => ServiceProvider.GetRequiredService<IResearcherRepository>();
        protected IPhDStudentRepository PhDStudentRepository => ServiceProvider.GetRequiredService<IPhDStudentRepository>();
        protected IResetPasswordRepository ResetPasswordRepository => ServiceProvider.GetRequiredService<IResetPasswordRepository>();
        protected IRefreshTokenRepository RefreshTokenRepository => ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
        protected IResearcherProfileService ResearcherProfileService => ServiceProvider.GetRequiredService<IResearcherProfileService>();
        protected IPhDStudentProfileService PhDStudentProfileService => ServiceProvider.GetRequiredService<IPhDStudentProfileService>();
        protected IMasterianProfileService MasterianProfileService => ServiceProvider.GetRequiredService<IMasterianProfileService>();
        protected IResearchAxisService ResearchAxisService => ServiceProvider.GetRequiredService<IResearchAxisService>();
        protected IResearchAxisRepository ResearchAxisRepository => ServiceProvider.GetRequiredService<IResearchAxisRepository>();
        protected IEventsRepository EventsRepository => ServiceProvider.GetRequiredService<IEventsRepository>();

        protected BaseTests()
        {
            var services = new ServiceCollection();

            // Add configuration with test settings
            var configurationData = new Dictionary<string, string>
            {
                { "Jwt:Key", "TestJwtSecretKeyForUnitTestsWithMinimumLength32Characters" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Jwt:ExpiryMinutes", "60" },
                { "RefreshToken:ExpiryDays", "7" },
                { "OTPToken:ExpiryMinutes", "10" },
                { "ResetPasswordToken:ExpiryMinutes", "30" },
                { "Email:SmtpServer", "localhost" },
                { "Email:SmtpPort", "587" },
                { "Email:SenderEmail", "test@example.com" },
                { "Email:SenderPassword", "password" },
                { "Email:EnableSSL", "false" },
                { "BlobStorage:ConnectionStringSecretName", "BlobStorageConnectionString" },
                { "KeyvaultUri", "https://test.vault.azure.net/" },
                { "DefaultConnection", "Server=localhost;Database=limtic_test;User Id=test;Password=test;" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            // Register infrastructure and application modules
            services.AddInfrastructure(configuration);
            services.AddApplication();
            services.AddMappers();
            services.AddWebApi();

            // In this environment, the default DataProtection key store under the user profile may be blocked.
            // Persist keys to a local test directory instead so crypto-based tests remain deterministic.
            var dpKeysPath = Path.Combine(Directory.GetCurrentDirectory(), "DataProtection-Keys");
            Directory.CreateDirectory(dpKeysPath);
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dpKeysPath));

            var emailDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IEmailService));
            if (emailDescriptor != null)
                services.Remove(emailDescriptor);
            services.AddScoped<IEmailService, FakeEmailService>();

            // Override IBlobStorageService with a mock implementation
            var blobDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IBlobStorageService));
            if (blobDescriptor != null)
                services.Remove(blobDescriptor);
            services.AddSingleton<IBlobStorageService, MockBlobStorageService>();

            // Override ICurrentUserService with a test stub that acts as SuperAdmin
            // so profile service authorization checks pass in unit tests
            services.AddScoped<ICurrentUserService, TestCurrentUserService>();

            // Remove previous AppDbContext registration if present
            var descriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Register AppDbContext with in-memory database
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("UnitTestDb"), ServiceLifetime.Scoped);

            ServiceProvider = services.BuildServiceProvider();
            DbContext = ServiceProvider.GetRequiredService<AppDbContext>();
            // Ensure in-memory database is created so model seeds (HasData) are applied
            DbContext.Database.EnsureCreated();
        }

        private sealed class FakeEmailService : IEmailService
        {
            public Task SendOTPEmailAsync(OTPEmailModel model) => Task.CompletedTask;
            public Task SendContactEmailAsync(ContactEmailModel model) => Task.CompletedTask;
            public Task<bool> TestSmtpAsync(string testEmail) => Task.FromResult(true);
        }

        private sealed class MockBlobStorageService : IBlobStorageService
        {
            private readonly Dictionary<string, byte[]> _storage = new();

            public Task<bool> UploadStreamAsync(Stream stream, string containerName, string destinationPath, bool overwrite)
            {
                var key = $"{containerName}/{destinationPath}";

                if (_storage.ContainsKey(key) && !overwrite)
                    return Task.FromResult(false);

                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    _storage[key] = memoryStream.ToArray();
                }

                return Task.FromResult(true);
            }

            public Task<Stream> GetStreamAsync(string containerName, string path)
            {
                var key = $"{containerName}/{path}";

                if (!_storage.ContainsKey(key))
                    throw new FileNotFoundException($"Blob '{key}' not found.");

                var stream = new MemoryStream(_storage[key]);
                return Task.FromResult<Stream>(stream);
            }
        }

        public void Dispose()
        {
            DbContext?.Dispose();
            if (ServiceProvider is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
