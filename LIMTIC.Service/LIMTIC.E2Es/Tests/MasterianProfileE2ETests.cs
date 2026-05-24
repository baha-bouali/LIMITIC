using System.Net;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;
using LIMTIC.WebAPI.Models.Profiles;
using LIMTIC.WebAPI.Models.UserManagement.CreateUser;
using LIMTIC.WebAPI.Models.UserManagement.UpdateUserRole;

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
            var createResponse = await Client.AddUser(new CreateUserRequest
            {
                FirstName = "Masterian",
                LastName = "E2E",
                Email = email,
                Password = "password",
                IsActive = true
            }, token);

            Assert.NotNull(createResponse);
            var userId = createResponse.User.Id;

            var roleResponse = await Client.UpdateUserRole(userId, new UpdateUserRoleRequest
            {
                Role = UserRole.Masterian,
                Cohort = cohort,
                DissertationSubject = dissertation
            }, token);

            Assert.True(roleResponse.IsSuccessStatusCode,
                $"UpdateRole failed: {await roleResponse.Content.ReadAsStringAsync()}");

            return userId;
        }

        // ── GET ────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetMasterianProfile_ExistingProfile_Returns200WithData()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateMasterianUserAsync("e2e.get.masterian@example.com", token, "2023", "Blockchain Security");

            var profile = await Client.GetMasterianProfile(userId, token);

            Assert.NotNull(profile);
            Assert.Equal(userId, profile!.Profile?.Id);
            Assert.Equal("2023", profile.Profile?.Cohort);
            Assert.Equal("Blockchain Security", profile.Profile?.DissertationSubject);
            Assert.Equal(UserRole.Masterian, profile.Profile?.Role);
        }

        [Fact]
        public async Task GetMasterianProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.GetMasterianProfileFullResponse(Guid.NewGuid(), token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetMasterianProfile_Unauthenticated_Returns401()
        {
            var response = await Client.GetMasterianProfileFullResponse(Guid.NewGuid(), "invalid-token");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── UPDATE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateMasterianProfile_ValidData_Returns200WithUpdatedFields()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateMasterianUserAsync("e2e.update.masterian@example.com", token);

            var updateRequest = new UpdateMasterianProfileRequest
            {
                Cohort = "2025",
                DissertationSubject = "Edge Computing Security"
            };

            var response = await Client.UpdateMasterianProfile(userId, updateRequest, token);

            Assert.True(response.IsSuccessStatusCode,
                $"Update failed: {await response.Content.ReadAsStringAsync()}");

            // Verify changes persisted
            var profile = await Client.GetMasterianProfile(userId, token);
            Assert.NotNull(profile?.Profile);
            Assert.Equal("2025", profile!.Profile!.Cohort);
            Assert.Equal("Edge Computing Security", profile.Profile.DissertationSubject);
        }

        [Fact]
        public async Task UpdateMasterianProfile_MissingRequiredFields_Returns400()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateMasterianUserAsync("e2e.update.masterian.invalid@example.com", token);

            var updateRequest = new UpdateMasterianProfileRequest
            {
                Cohort = "",                // required
                DissertationSubject = ""    // required
            };

            var response = await Client.UpdateMasterianProfile(userId, updateRequest, token);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateMasterianProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var updateRequest = new UpdateMasterianProfileRequest
            {
                Cohort = "2024",
                DissertationSubject = "Something"
            };

            var response = await Client.UpdateMasterianProfile(Guid.NewGuid(), updateRequest, token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateMasterianProfile_Unauthenticated_Returns401()
        {
            var updateRequest = new UpdateMasterianProfileRequest
            {
                Cohort = "2024",
                DissertationSubject = "Something"
            };

            var response = await Client.UpdateMasterianProfile(Guid.NewGuid(), updateRequest, "invalid-token");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── DELETE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteMasterianProfile_ExistingProfile_Returns200AndResetsRoleToVisitor()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateMasterianUserAsync("e2e.delete.masterian@example.com", token);

            var deleteResponse = await Client.DeleteMasterianProfile(userId, token);

            Assert.True(deleteResponse.IsSuccessStatusCode,
                $"Delete failed: {await deleteResponse.Content.ReadAsStringAsync()}");

            // Profile must be gone
            var profileResponse = await Client.GetMasterianProfileFullResponse(userId, token);
            Assert.Equal(HttpStatusCode.NotFound, profileResponse.StatusCode);

            // User role must be reset to Visitor
            var user = await Client.GetUserById(userId, token);
            Assert.NotNull(user);
            Assert.Equal(UserRole.Visitor, user!.User.Role);
        }

        [Fact]
        public async Task DeleteMasterianProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.DeleteMasterianProfile(Guid.NewGuid(), token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteMasterianProfile_NonAdmin_Returns403()
        {
            var adminToken = await LoginAsSuperAdmin();

            // Create a regular visitor user and get their token
            await Client.AddUser(new CreateUserRequest
            {
                FirstName = "Visitor",
                LastName = "User",
                Email = "e2e.visitor.delete.masterian@example.com",
                Password = "password",
                IsActive = true
            }, adminToken);

            var visitorToken = await Client.AuthenticateUser(
                new WebAPI.Models.Auth.Login.LoginRequest("e2e.visitor.delete.masterian@example.com", "password"));

            // Create a masterian to attempt to delete
            var masterianUserId = await CreateMasterianUserAsync("e2e.delete.masterian.target@example.com", adminToken);

            var response = await Client.DeleteMasterianProfile(masterianUserId, visitorToken?.AccessToken ?? "");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
