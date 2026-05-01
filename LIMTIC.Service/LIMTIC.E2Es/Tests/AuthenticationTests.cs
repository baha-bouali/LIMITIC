using LIMTIC.Application.Abstractions.Security;
using LIMTIC.Domain.Entities;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.WebAPI.Models.Auth.Login;
using Microsoft.Extensions.DependencyInjection;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class AuthenticationTests : BaseE2ETests
    {
        private IPasswordHasher PasswordHasher => Factory.Services
            .GetRequiredService<IPasswordHasher>();

        public AuthenticationTests(PostgresFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task AuthenticateUserE2ETest()
        {
            // Steps:
            // 1. Create a new user object with valid data
            // 2. Send a POST request to the API endpoint to add the user
            // 3. Assert that the response indicates success and the user was added
            // 4. Create a login request for the created user
            // 5. Send a POST request to the API endpoint to authenticate the user
            // 6. Assert that the response isn't null and the access token is not empty

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "John.Doe@example.com",
                PasswordHash = PasswordHasher.HashPassword("password"),
                Role = UserRole.Admin,
                AvatarBlobName = null,
                IsActive = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            var addResponse = await Client.AddUser(user);
            Assert.True(addResponse, "User added successfully");

            var loginRequest = new LoginRequest("John.Doe@example.com", "password");

            var authResponse = await Client.AuthenticateUser(loginRequest);
            Assert.NotNull(authResponse);
            Assert.NotEmpty(authResponse.AccessToken);
        }
    }
}
