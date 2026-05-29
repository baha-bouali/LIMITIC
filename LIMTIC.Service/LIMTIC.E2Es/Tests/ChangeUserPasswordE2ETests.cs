using LIMTIC.Application.Contracts.Commands.ChangeUserPassword;
using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class ChangeUserPasswordE2ETests : BaseE2ETests
    {
        public ChangeUserPasswordE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture) : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
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

            var createUserRequest = new CreateUserCommand
            {
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice.smith@example.com",
                Password = "OldPassword1!",
                IsActive = true,
            };

            var createResponse = await Client.AddUser(createUserRequest, superAdminAccessToken);
            Assert.NotNull(createResponse.Data);
            Assert.Null(createResponse.Message);

            var changePasswordRequest = new ChangeUserPasswordCommand
            {
                Email = createUserRequest.Email,
                OldPassword = "OldPassword1!",
                NewPassword = "NewPassword2!",
            };

            var changePasswordResponse = await Client.ChangePassword(changePasswordRequest, superAdminAccessToken);

            Assert.True(changePasswordResponse.Success);

            var retrievedUser = await Client.GetUserById(createResponse.Data.Id, superAdminAccessToken);
            Assert.NotNull(retrievedUser);
            Assert.Equal(createResponse.Data?.Id, retrievedUser.Data?.Id);
            Assert.Equal(createResponse.Data?.FirstName, retrievedUser.Data?.FirstName);
            Assert.Equal(createResponse.Data?.LastName, retrievedUser.Data?.LastName);
            Assert.Equal(createResponse.Data?.Email, retrievedUser.Data?.Email);
        }
    }
}
