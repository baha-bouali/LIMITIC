using LIMTIC.Application.DTOs.UserManagement.CreateUser;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.WebAPI.Models.Auth.Login;
using System.Net;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class AuthenticationE2ETests : BaseE2ETests
    {
        public AuthenticationE2ETests(PostgresFixture fixture) : base(fixture)
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

            string superAdminAccessToken = await LoginAsSuperAdmin();

            var user = new CreateUserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "John.Doe@example.com",
                Password = "password",
                Role = UserRole.Admin,
                IsActive = true
            };

            var addResponse = await Client.AddUser(user, superAdminAccessToken);
            Assert.NotNull(addResponse);

            var loginRequest = new LoginRequest(user.Email, user.Password);

            var authResponse = await Client.AuthenticateUser(loginRequest);
            Assert.NotNull(authResponse);
            Assert.NotNull(authResponse.AccessToken);
            Assert.NotEmpty(authResponse.AccessToken);
        }

        [Fact]
        public async Task AccessTokenE2ETest()
        {
            // Steps:
            // 1. Authenticate a user and obtain an access token and refresh token
            // 2. Wait for the access token to expire (simulate this by setting a short expiration time in the test environment)
            // 3. Attempt to access a protected resource with the expired access token and assert that it fails
            // 4. Send a POST request to the API endpoint to refresh the access token using the refresh token
            // 5. Assert that the response isn't null and contains a new access token

            // 1. Authenticate a user and obtain an access token and refresh token
            string superAdminAccessToken = await LoginAsSuperAdmin();

            // 2. Wait for the access token to expire (simulate this by setting a short expiration time in the test environment)
            await Task.Delay(TimeSpan.FromSeconds(3));

            // 3. Attempt to access a protected resource with the expired access token and assert that it fails
            var protectedResponse = await Client.GetUserByIdFullResponse(Guid.NewGuid(), superAdminAccessToken);
            Assert.Equal(HttpStatusCode.Unauthorized, protectedResponse.StatusCode);

            // 4. Send a POST request to the API endpoint to refresh the access token using the refresh token
            var refreshTokenReponse = await Client.RefreshToken();
            Assert.NotNull(refreshTokenReponse);
            Assert.NotNull(refreshTokenReponse.AccessToken);
            Assert.NotEmpty(refreshTokenReponse.AccessToken);
        }

        [Fact]
        public async Task RefreshTokenE2ETest()
        {
            // Steps:
            // 1. Authenticate a user and obtain an access token and refresh token
            // 2. Wait for the refresh token to expire (simulate this by setting a short expiration time in the test environment)
            // 3. Attempt to refresh the access token using the expired refresh token and assert that it fails

            // 1. Authenticate a user and obtain an access token and refresh token
            string superAdminAccessToken = await LoginAsSuperAdmin();

            // 2. Wait for the refresh token to expire (simulate this by setting a short expiration time in the test environment)
            await Task.Delay(TimeSpan.FromSeconds(10));

            // 3. Attempt to refresh the access token using the expired refresh token and assert that it fails
            var refreshTokenReponse = await Client.RefreshTokenFullResponse();
            Assert.Equal(HttpStatusCode.Unauthorized, refreshTokenReponse.StatusCode);
        }
    }
}