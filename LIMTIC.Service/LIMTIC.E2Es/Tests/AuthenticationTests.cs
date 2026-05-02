using LIMTIC.Application.Abstractions.Security;
using LIMTIC.Domain.Entities;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.WebAPI.Models.Auth.Login;
using LIMTIC.WebAPI.Models.UserManagement.CreateUser;
using Microsoft.Extensions.DependencyInjection;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class AuthenticationTests : BaseE2ETests
    {
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

            var user = new CreateUserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "John.Doe@example.com",
                Password = "password",
                Role = UserRole.Admin,
                IsActive = true
            };

            var addResponse = await Client.AddUser(user);
            Assert.NotNull(addResponse);

            var loginRequest = new LoginRequest(user.Email, user.Password);

            var authResponse = await Client.AuthenticateUser(loginRequest);
            Assert.NotNull(authResponse);
            Assert.NotNull(authResponse.AccessToken);
            Assert.NotEmpty(authResponse.AccessToken);
        }
    }
}
