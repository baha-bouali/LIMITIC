using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.WebAPI.Models.UserManagement.CreateUser;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class UsersManagementControllerE2ETests : BaseE2ETests
    {
        public UsersManagementControllerE2ETests(PostgresFixture fixture) : base(fixture)
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

            var createUserRequest = new CreateUserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PasswordHash = "hashedpassword",
                Role = UserRole.Admin,
                IsActive = true
            };

            var response = await Client.AddUser(createUserRequest);
            var createdUser = response.User;
            Assert.NotNull(createdUser);
            Assert.Null(response.Message);

            // 4. Send a GET request to the API endpoint to retrieve the user by ID
            var retrievedUser = await Client.GetUserById(response.User.Id);

            Assert.NotNull(retrievedUser);
            Assert.Equal(createdUser.Id, retrievedUser.User.Id);
            Assert.Equal(createdUser.FirstName, retrievedUser.User.FirstName);
            Assert.Equal(createdUser.LastName, retrievedUser.User.LastName);
            Assert.Equal(createdUser.Email, retrievedUser.User.Email);
        }
    }
}
