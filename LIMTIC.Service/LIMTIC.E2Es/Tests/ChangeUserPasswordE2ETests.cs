using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.WebAPI.Models.UserManagement.ChangeUserPassword;
using LIMTIC.WebAPI.Models.UserManagement.CreateUser;


namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class ChangeUserPasswordE2ETests : BaseE2ETests
    {
        public ChangeUserPasswordE2ETests(PostgresFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task ChangeUserPasswordE2ETest()
        {
            // Steps:
            // 1. Login as super admin and create a new user
            // 2. Send a POST request to change the user's password with the correct old password
            // 3. Assert that the response indicates success
            // 4. Send a GET request to retrieve the user and assert all other fields are unchanged

            string superAdminAccessToken = await LoginAsSuperAdmin();

            var createUserRequest = new CreateUserRequest
            {
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice.smith@example.com",
                Password = "OldPassword1!",
                Role = UserRole.Admin,
                IsActive = true,
            };

            var createResponse = await Client.AddUser(createUserRequest, superAdminAccessToken);
            Assert.NotNull(createResponse.User);
            Assert.Null(createResponse.Message);

            var changePasswordRequest = new ChangeUserPasswordRequest
            {
                Email = createUserRequest.Email,
                OldPassword = "OldPassword1!",
                NewPassword = "NewPassword2!",
            };

            var changePasswordResponse = await Client.ChangePassword(changePasswordRequest, superAdminAccessToken);

            Assert.True(changePasswordResponse.IsSuccessStatusCode);

            var retrievedUser = await Client.GetUserById(createResponse.User.Id, superAdminAccessToken);
            Assert.NotNull(retrievedUser);
            Assert.Equal(createResponse.User.Id, retrievedUser.User.Id);
            Assert.Equal(createResponse.User.FirstName, retrievedUser.User.FirstName);
            Assert.Equal(createResponse.User.LastName, retrievedUser.User.LastName);
            Assert.Equal(createResponse.User.Email, retrievedUser.User.Email);
        }
    }
}
