using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Commands.Login;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class BlobStorageE2ETests : BaseE2ETests
    {
        public BlobStorageE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture)
            : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
        {
        }

        [Fact]
        public async Task BlobStorageService_UploadsPdfE2ETest()
        {
            // Steps:
            // 1) Add a user
            // 2) Upload a PDF file as the user's avatar
            // 3) Assert that the upload was successful

            // 1) Add a user
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

            var login = await Client.AuthenticateUser(new LoginCommand(createUserCommand.Email, createUserCommand.Password));
            Assert.True(login.Success);

            // 2) Upload a jpg image as the user's avatar
            var avatarPath = Path.Combine(AppContext.BaseDirectory, "TestFiles", "avatar.jpg");
            Assert.True(File.Exists(avatarPath), $"Test file was not found: {avatarPath}");
            var avatarBytes = await File.ReadAllBytesAsync(avatarPath);

            var uploaded = await Client.UploadAvatar(
                userId: addResponse.Data.Id,
                fileBytes: avatarBytes,
                fileName: "avatar.jpg",
                accessToken: login.Data.AccessToken);

            Assert.True(uploaded.Success);
        }
    }
}
