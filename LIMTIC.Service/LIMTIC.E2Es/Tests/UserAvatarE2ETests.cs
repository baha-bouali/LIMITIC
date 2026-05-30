using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Commands.Login;
using LIMTIC.Application.Contracts.Commands.UpdateUserRole;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;
using System.Text;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class UserAvatarE2ETests : BaseE2ETests
    {
        public UserAvatarE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture)
            : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
        {
        }

        // ── Helpers ────────────────────────────────────────────────────────────────

        private static byte[] FakeJpeg() => Encoding.UTF8.GetBytes("fake-jpeg-content");

        private async Task<Guid> CreateResearcherAsync(string email, string token)
        {
            var created = await Client.AddUser(new CreateUserCommand
            {
                FirstName = "Avatar",
                LastName = "Researcher",
                Email = email,
                Password = "password",
                IsActive = true
            }, token);

            await Client.UpdateUserRole(new UpdateUserRoleCommand
            {
                UserId = created.Data.Id,
                Role = UserRole.Researcher,
                Rank = "Dr",
                Specialty = "AI",
                Office = "B1",
                PhoneNumber = "11111",
                ResearchAxisIds = []
            }, token);

            return created.Data.Id;
        }

        // ── Upload ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task UploadAvatar_AsAdmin_Returns200AndUpdatesAvatarBlobName()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateResearcherAsync("e2e.avatar.upload@example.com", token);

            var response = await Client.UploadAvatar(userId, FakeJpeg(), "photo.jpg", token);

            Assert.True(response.Success);

            // Verify the profile DTO now carries the updated AvatarBlobName
            var profile = await Client.GetResearcherProfile(userId);
            Assert.NotNull(profile?.Data?.AvatarBlobName);
            Assert.Contains("photo.jpg", profile!.Data!.AvatarBlobName);
        }

        [Fact]
        public async Task UploadAvatar_Unauthenticated_Returns401()
        {
            var response = await Client.UploadAvatar(
                Guid.NewGuid(), FakeJpeg(), "photo.jpg", "invalid-token");

            Assert.False(response.Success);
        }

        [Fact]
        public async Task UploadAvatar_NonAdmin_ForAnotherUser_Returns403()
        {
            var adminToken = await LoginAsSuperAdmin();

            // Create a researcher (target)
            var targetId = await CreateResearcherAsync("e2e.avatar.target@example.com", adminToken);

            // Create a visitor (the attacker)
            await Client.AddUser(new CreateUserCommand
            {
                FirstName = "V", LastName = "U",
                Email = "e2e.avatar.visitor@example.com",
                Password = "password", IsActive = true
            }, adminToken);

            var visitorToken = await Client.AuthenticateUser(
                new LoginCommand("e2e.avatar.visitor@example.com", "password"));

            var response = await Client.UploadAvatar(
                targetId, FakeJpeg(), "evil.jpg", visitorToken?.Data?.AccessToken ?? "");

            Assert.False(response.Success);
        }

        [Fact]
        public async Task UploadAvatar_SecondUpload_OverwritesBlobName()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateResearcherAsync("e2e.avatar.overwrite@example.com", token);

            await Client.UploadAvatar(userId, FakeJpeg(), "first.jpg", token);
            await Client.UploadAvatar(userId, FakeJpeg(), "second.jpg", token);

            var profile = await Client.GetResearcherProfile(userId);
            Assert.Contains("second.jpg", profile?.Data?.AvatarBlobName);
        }

        [Fact]
        public async Task UploadAvatar_VisitorUser_CanUploadOwnAvatar()
        {
            var adminToken = await LoginAsSuperAdmin();

            // Create a visitor and log in as that user
            var created = await Client.AddUser(new CreateUserCommand
            {
                FirstName = "Self",
                LastName = "Upload",
                Email = "e2e.avatar.self@example.com",
                Password = "password",
                IsActive = true
            }, adminToken);

            var userToken = await Client.AuthenticateUser(
                new LoginCommand("e2e.avatar.self@example.com", "password"));

            // User uploads their own avatar — should succeed
            var response = await Client.UploadAvatar(
                created!.Data.Id, FakeJpeg(), "self.jpg", userToken?.Data.AccessToken ?? "");

            Assert.True(response.Success);
        }
    }
}
