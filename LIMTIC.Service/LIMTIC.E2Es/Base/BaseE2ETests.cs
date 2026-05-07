using LIMTIC.Application.Abstractions.Security;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;
using LIMTIC.Infrastructure.Data;
using LIMTIC.WebAPI.Models.Auth.Login;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LIMTIC.E2Es.Base
{
    public class BaseE2ETests : IClassFixture<PostgresFixture>
    {
        protected readonly HttpClient Client;
        protected readonly CustomWebApplicationFactory Factory;
        protected IPasswordHasher PasswordHasher => Factory.Services
            .GetRequiredService<IPasswordHasher>();

        public BaseE2ETests(PostgresFixture fixture)
        {
            Factory = new CustomWebApplicationFactory(fixture.ConnectionString);
            Client = Factory.CreateClient();

            InitializeDatabase();
        }

        public BaseE2ETests(PostgresFixture postgresFixture, MailHogFixture mailHogFixture)
        {
            Factory = new CustomWebApplicationFactory(
                postgresFixture.ConnectionString,
                mailHogFixture.SmtpPort);
            Client = Factory.CreateClient();

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
            SeedAdminUser(db);
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

        protected async Task<string> LoginAsSuperAdmin()
        {
            var loginRequest = new LoginRequest("admin@test.com", "AdminPassword");
            var authResponse = await Client.AuthenticateUser(loginRequest);

            return authResponse.AccessToken;
        }
    }
}