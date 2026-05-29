using System.Net;
using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Commands.Login;
using LIMTIC.Application.Contracts.Commands.Profiles;
using LIMTIC.Application.Contracts.Commands.UpdateUserRole;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class MasterianProfileE2ETests : BaseE2ETests
    {
        public MasterianProfileE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture)
            : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
        {
        }

        // ── Helpers ────────────────────────────────────────────────────────────────

        private async Task<Guid> CreateMasterianUserAsync(
            string email, string token,
            string cohort = "2024", string dissertation = "Initial Subject")
        {
            var createResponse = await Client.AddUser(new CreateUserCommand
            {
                FirstName = "Masterian",
                LastName = "E2E",
                Email = email,
                Password = "password",
                IsActive = true
            }, token);

            Assert.NotNull(createResponse);
            var userId = createResponse.Data.Id;

            var roleResponse = await Client.UpdateUserRole(userId, new UpdateUserRoleCommand
            {
                Role = UserRole.Masterian,
                Cohort = cohort,
                DissertationSubject = dissertation
            }, token);

            Assert.True(roleResponse.Success);

            return userId;
        }

        // ── GET ────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetMasterianProfile_ExistingProfile_Returns200WithData()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateMasterianUserAsync("e2e.get.masterian@example.com", token, "2023", "Blockchain Security");

            var profile = await Client.GetMasterianProfile(userId);

            Assert.NotNull(profile);
            Assert.Equal(userId, profile!.Data?.Id);
            Assert.Equal("2023", profile.Data?.Cohort);
            Assert.Equal("Blockchain Security", profile.Data?.DissertationSubject);
            Assert.Equal(UserRole.Masterian, profile.Data?.Role);
        }

        // ── UPDATE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateMasterianProfile_ValidData_Returns200WithUpdatedFields()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateMasterianUserAsync("e2e.update.masterian@example.com", token);

            var updateRequest = new UpdateMasterianProfileCommand
            {
                Cohort = "2025",
                DissertationSubject = "Edge Computing Security"
            };

            var response = await Client.UpdateMasterianProfile(userId, updateRequest, token);

            Assert.True(response.Success);

            // Verify changes persisted
            var profile = await Client.GetMasterianProfile(userId);
            Assert.NotNull(profile?.Data);
            Assert.Equal("2025", profile!.Data!.Cohort);
            Assert.Equal("Edge Computing Security", profile.Data.DissertationSubject);
        }

        [Fact]
        public async Task UpdateMasterianProfile_MissingRequiredFields_Returns400()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateMasterianUserAsync("e2e.update.masterian.invalid@example.com", token);

            var updateRequest = new UpdateMasterianProfileCommand
            {
                Cohort = "",                // required
                DissertationSubject = ""    // required
            };

            var response = await Client.UpdateMasterianProfile(userId, updateRequest, token);

            Assert.False(response.Success);
        }

        [Fact]
        public async Task UpdateMasterianProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var updateRequest = new UpdateMasterianProfileCommand
            {
                Cohort = "2024",
                DissertationSubject = "Something"
            };

            var response = await Client.UpdateMasterianProfile(Guid.NewGuid(), updateRequest, token);

            Assert.False(response.Success);
        }

        [Fact]
        public async Task UpdateMasterianProfile_Unauthenticated_Returns401()
        {
            var updateRequest = new UpdateMasterianProfileCommand
            {
                Cohort = "2024",
                DissertationSubject = "Something"
            };

            var response = await Client.UpdateMasterianProfile(Guid.NewGuid(), updateRequest, "invalid-token");

            Assert.False(response.Success);
        }

        // ── DELETE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteMasterianProfile_ExistingProfile_Returns200AndResetsRoleToVisitor()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateMasterianUserAsync("e2e.delete.masterian@example.com", token);

            var deleteResponse = await Client.DeleteMasterianProfile(userId, token);

            Assert.True(deleteResponse.Success);

            // Profile must be gone
            var profileResponse = await Client.GetMasterianProfile(userId);
            Assert.False(profileResponse.Success);

            // User role must be reset to Visitor
            var user = await Client.GetUserById(userId, token);
            Assert.NotNull(user);
            Assert.Equal(UserRole.Visitor, user!.Data.Role);
        }

        [Fact]
        public async Task DeleteMasterianProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.DeleteMasterianProfile(Guid.NewGuid(), token);

            Assert.False(response.Success);
        }

        [Fact]
        public async Task DeleteMasterianProfile_NonAdmin_Returns403()
        {
            var adminToken = await LoginAsSuperAdmin();

            // Create a regular visitor user and get their token
            await Client.AddUser(new CreateUserCommand
            {
                FirstName = "Visitor",
                LastName = "User",
                Email = "e2e.visitor.delete.masterian@example.com",
                Password = "password",
                IsActive = true
            }, adminToken);

            var visitorToken = await Client.AuthenticateUser(new LoginCommand("e2e.visitor.delete.masterian@example.com", "password"));

            // Create a masterian to attempt to delete
            var masterianUserId = await CreateMasterianUserAsync("e2e.delete.masterian.target@example.com", adminToken);

            var response = await Client.DeleteMasterianProfile(masterianUserId, visitorToken?.Data?.AccessToken ?? "");

            Assert.False(response.Success);
        }
    }
}
