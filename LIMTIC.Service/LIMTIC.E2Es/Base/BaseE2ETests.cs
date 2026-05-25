using LIMTIC.Application.Abstractions.Security;
using LIMTIC.Application.Abstractions.Storage;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.HttpCookie;
using LIMTIC.E2Es.MailFixture;
using LIMTIC.Infrastructure.Data;
using LIMTIC.WebAPI.Models.Auth.Login;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace LIMTIC.E2Es.Base
{
    public class BaseE2ETests : IClassFixture<PostgresFixture>, IClassFixture<MailHogFixture>
    {
        protected readonly HttpClient Client;
        protected readonly CustomWebApplicationFactory Factory;
        protected IPasswordHasher PasswordHasher => Factory.Services
            .GetRequiredService<IPasswordHasher>();
        protected IBlobStorageService BlobStorageService => Factory.Services
           .GetRequiredService<IBlobStorageService>();

        public BaseE2ETests(PostgresFixture dbfixture, MailHogFixture mailHogFixture, bool useShortTokenExpiry = false)
        {
            var overrides = useShortTokenExpiry
            ? new Dictionary<string, string?>
            {
                ["Jwt:ExpireInMinutes"] = "0",
                ["Jwt:ExpireInSeconds"] = "3",
                ["RefreshToken:ExpireInDays"] = "0",
                ["RefreshToken:ExpireInSeconds"] = "10"
            }
            : new Dictionary<string, string?>();

            Factory = new CustomWebApplicationFactory(
                connectionString: dbfixture.ConnectionString, 
                configOverrides: overrides, 
                mailHogSmtpPort: mailHogFixture.SmtpPort); 

            // Configure HttpClient with CookieHandler to manage cookies across requests
            Client = Factory.CreateDefaultClient(new Uri("https://localhost"), new CookieHandler(new CookieContainer()));

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
            ResetDatabase(db);
            SeedAdminUser(db);
        }

        private static void ResetDatabase(AppDbContext db)
        {
            // Ensure each E2E test starts with a clean database state.
            // xUnit creates a new test class instance per test, but the Postgres container is shared.
            db.Database.ExecuteSqlRaw(
                """
                TRUNCATE TABLE
                    public."RefreshTokens",
                    public."ResetPasswords",
                    public."Speakers",
                    public."Events",
                    public."ResearchAxes",
                    public."Researchers",
                    public."PhDStudents",
                    public."Masterians",
                    public."BookChapters",
                    public."InternationalConferences",
                    public."NationalConferences",
                    public."TechnicalReports",
                    public."JournalArticles",
                    public."Publications",
                    public."Users"
                RESTART IDENTITY CASCADE;
                """);
        }

        private void SeedAdminUser(AppDbContext db)
        {
            // Avoid duplicate seeding across test runs
            if (db.Users.Any(u => u.Email == "admin@test.com"))
                return;

            db.Users.Add(new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin",
                LastName = "Admin",
                IsActive = true,
                Email = "admin@test.com",
                Role = UserRole.SuperAdmin,
                PasswordHash = PasswordHasher.HashPassword("AdminPassword"),
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = Guid.Empty
            });

            db.SaveChanges();
        }

        protected async Task<string?> LoginAsSuperAdmin()
        {
            var loginRequest = new LoginRequest("admin@test.com", "AdminPassword");
            var authResponse = await Client.AuthenticateUser(loginRequest);

            return authResponse?.AccessToken;
        }
    }
}
