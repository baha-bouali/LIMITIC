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
    public class ResearcherProfileE2ETests : BaseE2ETests
    {
        public ResearcherProfileE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture)
            : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
        {
        }

        // ── Helpers ────────────────────────────────────────────────────────────────

        private async Task<Guid> CreateResearcherUserAsync(string email, string token)
        {
            var createResponse = await Client.AddUser(new CreateUserRequest
            {
                FirstName = "Researcher",
                LastName = "E2E",
                Email = email,
                Password = "password",
                IsActive = true
            }, token);

            Assert.NotNull(createResponse);
            var userId = createResponse.User.Id;

            var roleResponse = await Client.UpdateUserRole(userId, new UpdateUserRoleRequest
            {
                Role = UserRole.Researcher,
                Rank = "Professor",
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345",
                ResearchAxisIds = new List<Guid>()
            }, token);

            Assert.True(roleResponse.IsSuccessStatusCode,
                $"UpdateRole failed: {await roleResponse.Content.ReadAsStringAsync()}");

            return userId;
        }

        // ── GET ────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetResearcherProfile_ExistingProfile_Returns200WithData()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateResearcherUserAsync("e2e.get.researcher@example.com", token);

            var profile = await Client.GetResearcherProfile(userId, token);

            Assert.NotNull(profile);
            Assert.Equal(userId, profile!.Profile?.Id);
            Assert.Equal("Professor", profile.Profile?.Rank);
            Assert.Equal("AI", profile.Profile?.Specialty);
            Assert.Equal(UserRole.Researcher, profile.Profile?.Role);
        }

        [Fact]
        public async Task GetResearcherProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.GetResearcherProfileFullResponse(Guid.NewGuid(), token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetResearcherProfile_Unauthenticated_Returns401()
        {
            var response = await Client.GetResearcherProfileFullResponse(Guid.NewGuid(), "invalid-token");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── UPDATE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateResearcherProfile_ValidData_Returns200WithUpdatedFields()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateResearcherUserAsync("e2e.update.researcher@example.com", token);

            var updateRequest = new UpdateResearcherProfileRequest
            {
                Rank = "Full Professor",
                Specialty = "Deep Learning",
                Office = "Room 42",
                PhoneNumber = "99999",
                Biography = "Expert in DL",
                LinkedIn = "https://linkedin.com/test"
            };

            var response = await Client.UpdateResearcherProfile(userId, updateRequest, token);

            Assert.True(response.IsSuccessStatusCode,
                $"Update failed: {await response.Content.ReadAsStringAsync()}");

            // Verify changes persisted
            var profile = await Client.GetResearcherProfile(userId, token);
            Assert.NotNull(profile?.Profile);
            Assert.Equal("Full Professor", profile!.Profile!.Rank);
            Assert.Equal("Deep Learning", profile.Profile.Specialty);
            Assert.Equal("Expert in DL", profile.Profile.Biography);
            Assert.Equal("https://linkedin.com/test", profile.Profile.LinkedIn);
        }

        [Fact]
        public async Task UpdateResearcherProfile_MissingRequiredFields_Returns400()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateResearcherUserAsync("e2e.update.researcher.invalid@example.com", token);

            var updateRequest = new UpdateResearcherProfileRequest
            {
                Rank = "",          // required
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345"
            };

            var response = await Client.UpdateResearcherProfile(userId, updateRequest, token);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateResearcherProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var updateRequest = new UpdateResearcherProfileRequest
            {
                Rank = "Prof",
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345"
            };

            var response = await Client.UpdateResearcherProfile(Guid.NewGuid(), updateRequest, token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateResearcherProfile_Unauthenticated_Returns401()
        {
            var updateRequest = new UpdateResearcherProfileRequest
            {
                Rank = "Prof",
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345"
            };

            var response = await Client.UpdateResearcherProfile(Guid.NewGuid(), updateRequest, "invalid-token");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── DELETE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteResearcherProfile_ExistingProfile_Returns200AndResetsRoleToVisitor()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateResearcherUserAsync("e2e.delete.researcher@example.com", token);

            var deleteResponse = await Client.DeleteResearcherProfile(userId, token);

            Assert.True(deleteResponse.IsSuccessStatusCode,
                $"Delete failed: {await deleteResponse.Content.ReadAsStringAsync()}");

            // Profile must be gone
            var profileResponse = await Client.GetResearcherProfileFullResponse(userId, token);
            Assert.Equal(HttpStatusCode.NotFound, profileResponse.StatusCode);

            // User role must be reset to Visitor
            var user = await Client.GetUserById(userId, token);
            Assert.NotNull(user);
            Assert.Equal(UserRole.Visitor, user!.User.Role);
        }

        [Fact]
        public async Task DeleteResearcherProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.DeleteResearcherProfile(Guid.NewGuid(), token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteResearcherProfile_NonAdmin_Returns403()
        {
            var adminToken = await LoginAsSuperAdmin();

            // Create a regular visitor user and get their token
            var visitorResponse = await Client.AddUser(new CreateUserRequest
            {
                FirstName = "Visitor",
                LastName = "User",
                Email = "e2e.visitor.delete.researcher@example.com",
                Password = "password",
                IsActive = true
            }, adminToken);

            Assert.NotNull(visitorResponse);

            // Login as that visitor
            var visitorToken = await Client.AuthenticateUser(
                new WebAPI.Models.Auth.Login.LoginRequest("e2e.visitor.delete.researcher@example.com", "password"));

            // Create a researcher to attempt to delete
            var researcherUserId = await CreateResearcherUserAsync("e2e.delete.researcher.target@example.com", adminToken);

            var response = await Client.DeleteResearcherProfile(researcherUserId, visitorToken?.AccessToken ?? "");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
