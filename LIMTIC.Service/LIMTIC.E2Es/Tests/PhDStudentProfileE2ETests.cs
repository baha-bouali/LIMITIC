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
    public class PhDStudentProfileE2ETests : BaseE2ETests
    {
        public PhDStudentProfileE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture)
            : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
        {
        }

        // ── Helpers ────────────────────────────────────────────────────────────────

        private async Task<Guid> CreatePhDStudentUserAsync(string email, string token, int enrollmentYear = 2022)
        {
            var createResponse = await Client.AddUser(new CreateUserRequest
            {
                FirstName = "PhDStudent",
                LastName = "E2E",
                Email = email,
                Password = "password",
                IsActive = true
            }, token);

            Assert.NotNull(createResponse);
            var userId = createResponse.User.Id;

            var roleResponse = await Client.UpdateUserRole(userId, new UpdateUserRoleRequest
            {
                Role = UserRole.PhDStudent,
                EnrollmentYear = enrollmentYear
            }, token);

            Assert.True(roleResponse.IsSuccessStatusCode,
                $"UpdateRole failed: {await roleResponse.Content.ReadAsStringAsync()}");

            return userId;
        }

        // ── GET ────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetPhDStudentProfile_ExistingProfile_Returns200WithData()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreatePhDStudentUserAsync("e2e.get.phd@example.com", token, 2021);

            var profile = await Client.GetPhDStudentProfile(userId, token);

            Assert.NotNull(profile);
            Assert.Equal(userId, profile!.Profile?.Id);
            Assert.Equal(2021, profile.Profile?.EnrollmentYear);
            Assert.Equal(UserRole.PhDStudent, profile.Profile?.Role);
        }

        [Fact]
        public async Task GetPhDStudentProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.GetPhDStudentProfileFullResponse(Guid.NewGuid(), token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetPhDStudentProfile_Unauthenticated_Returns401()
        {
            var response = await Client.GetPhDStudentProfileFullResponse(Guid.NewGuid(), "invalid-token");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── UPDATE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdatePhDStudentProfile_ValidData_Returns200WithUpdatedFields()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreatePhDStudentUserAsync("e2e.update.phd@example.com", token, 2022);

            var updateRequest = new UpdatePhDStudentProfileRequest
            {
                EnrollmentYear = 2023,
                ThesisSubject = "Federated Learning"
            };

            var response = await Client.UpdatePhDStudentProfile(userId, updateRequest, token);

            Assert.True(response.IsSuccessStatusCode,
                $"Update failed: {await response.Content.ReadAsStringAsync()}");

            // Verify changes persisted
            var profile = await Client.GetPhDStudentProfile(userId, token);
            Assert.NotNull(profile?.Profile);
            Assert.Equal(2023, profile!.Profile!.EnrollmentYear);
            Assert.Equal("Federated Learning", profile.Profile.ThesisSubject);
        }

        [Fact]
        public async Task UpdatePhDStudentProfile_InvalidEnrollmentYear_Returns400()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreatePhDStudentUserAsync("e2e.update.phd.invalid@example.com", token);

            var updateRequest = new UpdatePhDStudentProfileRequest
            {
                EnrollmentYear = 0   // must be > 0
            };

            var response = await Client.UpdatePhDStudentProfile(userId, updateRequest, token);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePhDStudentProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var updateRequest = new UpdatePhDStudentProfileRequest
            {
                EnrollmentYear = 2022
            };

            var response = await Client.UpdatePhDStudentProfile(Guid.NewGuid(), updateRequest, token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePhDStudentProfile_Unauthenticated_Returns401()
        {
            var updateRequest = new UpdatePhDStudentProfileRequest
            {
                EnrollmentYear = 2022
            };

            var response = await Client.UpdatePhDStudentProfile(Guid.NewGuid(), updateRequest, "invalid-token");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── DELETE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeletePhDStudentProfile_ExistingProfile_Returns200AndResetsRoleToVisitor()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreatePhDStudentUserAsync("e2e.delete.phd@example.com", token);

            var deleteResponse = await Client.DeletePhDStudentProfile(userId, token);

            Assert.True(deleteResponse.IsSuccessStatusCode,
                $"Delete failed: {await deleteResponse.Content.ReadAsStringAsync()}");

            // Profile must be gone
            var profileResponse = await Client.GetPhDStudentProfileFullResponse(userId, token);
            Assert.Equal(HttpStatusCode.NotFound, profileResponse.StatusCode);

            // User role must be reset to Visitor
            var user = await Client.GetUserById(userId, token);
            Assert.NotNull(user);
            Assert.Equal(UserRole.Visitor, user!.User.Role);
        }

        [Fact]
        public async Task DeletePhDStudentProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.DeletePhDStudentProfile(Guid.NewGuid(), token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeletePhDStudentProfile_NonAdmin_Returns403()
        {
            var adminToken = await LoginAsSuperAdmin();

            // Create a regular visitor user and get their token
            await Client.AddUser(new CreateUserRequest
            {
                FirstName = "Visitor",
                LastName = "User",
                Email = "e2e.visitor.delete.phd@example.com",
                Password = "password",
                IsActive = true
            }, adminToken);

            var visitorToken = await Client.AuthenticateUser(
                new WebAPI.Models.Auth.Login.LoginRequest("e2e.visitor.delete.phd@example.com", "password"));

            // Create a PhD student to attempt to delete
            var phdUserId = await CreatePhDStudentUserAsync("e2e.delete.phd.target@example.com", adminToken);

            var response = await Client.DeletePhDStudentProfile(phdUserId, visitorToken?.AccessToken ?? "");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
