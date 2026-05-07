using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.WebAPI.Models.UserManagement.CreateUser;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class UsersManagementE2ETests : BaseE2ETests
    {
        public UsersManagementE2ETests(PostgresFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task AddAndGetUserE2ETest()
        {
            // Steps:
            // 1. Create a new user object with valid data
            // 2. Send a POST request to the API endpoint to add the user
            // 3. Assert that the response indicates success and the user was added
            // 4. Send a GET request to the API endpoint to retrieve the user by ID

            string superAdminAccessToken = await LoginAsSuperAdmin();

            var createUserRequest = new CreateUserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "password",
                Role = UserRole.Admin,
                IsActive = true
            };

            var response = await Client.AddUser(createUserRequest, superAdminAccessToken);
            var createdUser = response.User;
            Assert.NotNull(createdUser);
            Assert.Null(response.Message);

            // 4. Send a GET request to the API endpoint to retrieve the user by ID
            var retrievedUser = await Client.GetUserById(response.User.Id, superAdminAccessToken);

            Assert.NotNull(retrievedUser);
            Assert.Equal(createdUser.Id, retrievedUser.User.Id);
            Assert.Equal(createdUser.FirstName, retrievedUser.User.FirstName);
            Assert.Equal(createdUser.LastName, retrievedUser.User.LastName);
            Assert.Equal(createdUser.Email, retrievedUser.User.Email);
        }

        [Fact]
        public async Task ActivateAndDeactivateUserE2ETest()
        {
            // Steps:
            // 1. Login as super admin
            // 2. Create a new user
            // 3. Activate the user
            // 4. Assert the user is active
            // 5. Deactivate the user
            // 6. Assert the user is inactive

            // 1. Login as super admin
            string superAdminAccessToken = await LoginAsSuperAdmin();

            // 2. Create a new user
            var createUserRequest = new CreateUserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe12@example.com",
                Password = "password",
                Role = UserRole.Researcher,
                IsActive = false
            };

            var createResponse = await Client.AddUser(createUserRequest, superAdminAccessToken);
            var createdUser = createResponse.User;
            Assert.NotNull(createdUser);
            Assert.Null(createResponse.Message);

            // 3. Activate the user
            var activateResponse = await Client.ActivateUser(createdUser.Id, superAdminAccessToken);
            Assert.NotNull(activateResponse);
            Assert.Null(activateResponse.Message);
            Assert.True(activateResponse.Success);

            // 4. Assert the user is active
            var activeUser = await Client.GetUserById(createdUser.Id, superAdminAccessToken);
            Assert.NotNull(activeUser);
            Assert.True(activeUser.User.IsActive);

            // 5. Deactivate the user
            var deactivateResponse = await Client.DeactivateUser(createdUser.Id, superAdminAccessToken);
            Assert.NotNull(deactivateResponse);
            Assert.Null(deactivateResponse.Message);
            Assert.True(deactivateResponse.Success);

            // 6. Assert the user is inactive
            var inactiveUser = await Client.GetUserById(createdUser.Id, superAdminAccessToken);
            Assert.NotNull(inactiveUser);
            Assert.False(inactiveUser.User.IsActive);
        }
    }
}