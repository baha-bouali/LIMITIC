using System.Net;
using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Commands.Login;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class AuthenticationE2ETests : BaseE2ETests
    {
        public AuthenticationE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture) 
            : base(dbfixture: dbfixture, mailHogFixture: mailFixture, useShortTokenExpiry: true)
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

            var createUserCommand = new CreateUserCommand
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "password",
                IsActive = true
            };

            var addResponse = await Client.AddUser(createUserCommand, superAdminAccessToken);
            Assert.NotNull(addResponse);
            Assert.True(addResponse.Success);

            var loginCommand = new LoginCommand(createUserCommand.Email, createUserCommand.Password);

            var authResponse = await Client.AuthenticateUser(loginCommand);
            Assert.NotNull(authResponse);
            Assert.NotNull(authResponse.Data.AccessToken);
            Assert.NotEmpty(authResponse.Data.AccessToken);
        }

        [Fact]
        public async Task AccessTokenE2ETest()
        {
            // Steps:
            // 1. Authenticate a user and obtain an access token and refresh token
            // 2. Wait for the access token to expire (simulate this by setting a short expiration time in the test environment)
            // 3. Attempt to access a protected resource with the expired access token and assert that it fails
            // 4. Send a POST request to the API endpoint to refresh the access token using the refresh token

            // 1. Authenticate a user and obtain a access token and refresh token
            string superAdminAccessToken = await LoginAsSuperAdmin();

            // 2. Wait for the access token to expire (simulate this by setting a short expiration time in the test environment)
            await Task.Delay(TimeSpan.FromSeconds(3));

            // 3. Attempt to access a protected resource with the expired access token and assert that it fails
            var protectedResponse = await Client.GetUserById(Guid.NewGuid(), superAdminAccessToken);
            Assert.False(protectedResponse.Success);

            // 4. Send a POST request to the API endpoint to refresh the access token using the refresh token
            var refreshTokenReponse = await Client.RefreshToken();
            Assert.NotNull(refreshTokenReponse);
            Assert.NotNull(refreshTokenReponse.Data.AccessToken);
            Assert.NotEmpty(refreshTokenReponse.Data.AccessToken);
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
            var refreshTokenReponse = await Client.RefreshToken();
            Assert.False(refreshTokenReponse.Success);
        }
    }
}