using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;
using LIMTIC.WebAPI.Models.UserManagement.CreateUser;
using LIMTIC.WebAPI.Models.UserManagement.UpdateUserRole;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class UpdateUserRoleE2ETests : BaseE2ETests
    {
        public UpdateUserRoleE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture)
            : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
        {
        }

        // ── Helpers ────────────────────────────────────────────────────────────────

        private async Task<Guid> CreateVisitorUserAsync(string email, string token)
        {
            var createResponse = await Client.AddUser(new CreateUserRequest
            {
                FirstName = "E2E",
                LastName = "User",
                Email = email,
                Password = "password",
                IsActive = true
            }, token);

            Assert.NotNull(createResponse);
            return createResponse.User.Id;
        }

        // ── Basic role assignments ─────────────────────────────────────────────────

        [Fact]
        public async Task UpdateUserRole_ToResearcher_E2E()
        {
            var token = await LoginAsSuperAdmin();

            var createUserRequest = new CreateUserRequest
            {
                FirstName = "E2E",
                LastName = "Research",
                Email = "e2e.research@example.com",
                Password = "password",
                IsActive = true
            };

            var createResponse = await Client.AddUser(createUserRequest, token);
            Assert.NotNull(createResponse);

            var updateRequest = new UpdateUserRoleRequest
            {
                Role = UserRole.Researcher,
                Rank = "Professor",
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345",
                ResearchAxisIds = new List<Guid>()
            };

            // ensure authorization header is set for the PUT request
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await Client.PutAsJsonAsync($"api/users/updateRole/{createResponse.User.Id}", updateRequest);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task UpdateUserRole_ToPhDStudent_E2E()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateVisitorUserAsync("e2e.role.phd@example.com", token);

            var response = await Client.UpdateUserRole(userId, new UpdateUserRoleRequest
            {
                Role = UserRole.PhDStudent,
                EnrollmentYear = 2023
            }, token);

            Assert.True(response.IsSuccessStatusCode,
                $"UpdateRole failed: {await response.Content.ReadAsStringAsync()}");

            // Verify PhD profile was created
            var profile = await Client.GetPhDStudentProfile(userId, token);
            Assert.NotNull(profile?.Profile);
            Assert.Equal(2023, profile!.Profile!.EnrollmentYear);
        }

        [Fact]
        public async Task UpdateUserRole_ToMasterian_E2E()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateVisitorUserAsync("e2e.role.masterian@example.com", token);

            var response = await Client.UpdateUserRole(userId, new UpdateUserRoleRequest
            {
                Role = UserRole.Masterian,
                Cohort = "2024",
                DissertationSubject = "Subject X"
            }, token);

            Assert.True(response.IsSuccessStatusCode,
                $"UpdateRole failed: {await response.Content.ReadAsStringAsync()}");

            // Verify Masterian profile was created
            var profile = await Client.GetMasterianProfile(userId, token);
            Assert.NotNull(profile?.Profile);
            Assert.Equal("2024", profile!.Profile!.Cohort);
        }

        // ── Role-switch scenarios ──────────────────────────────────────────────────

        [Fact]
        public async Task UpdateUserRole_ResearcherToMasterian_DeletesResearcherAndCreatesMasterian()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateVisitorUserAsync("e2e.switch.researcher.to.masterian@example.com", token);

            // Promote to Researcher
            var r1 = await Client.UpdateUserRole(userId, new UpdateUserRoleRequest
            {
                Role = UserRole.Researcher,
                Rank = "Associate",
                Specialty = "ML",
                Office = "Lab A",
                PhoneNumber = "99999",
                ResearchAxisIds = new List<Guid>()
            }, token);
            Assert.True(r1.IsSuccessStatusCode);

            // Now promote to Masterian — should delete Researcher row
            var r2 = await Client.UpdateUserRole(userId, new UpdateUserRoleRequest
            {
                Role = UserRole.Masterian,
                Cohort = "2025",
                DissertationSubject = "Deep Learning"
            }, token);
            Assert.True(r2.IsSuccessStatusCode,
                $"Role switch failed: {await r2.Content.ReadAsStringAsync()}");

            // Researcher profile must be gone
            var researcherResponse = await Client.GetResearcherProfileFullResponse(userId, token);
            Assert.Equal(HttpStatusCode.NotFound, researcherResponse.StatusCode);

            // Masterian profile must exist
            var masterianProfile = await Client.GetMasterianProfile(userId, token);
            Assert.NotNull(masterianProfile?.Profile);
            Assert.Equal("2025", masterianProfile!.Profile!.Cohort);
        }

        [Fact]
        public async Task UpdateUserRole_PhDStudentToResearcher_DeletesPhDAndCreatesResearcher()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateVisitorUserAsync("e2e.switch.phd.to.researcher@example.com", token);

            // Promote to PhDStudent
            var r1 = await Client.UpdateUserRole(userId, new UpdateUserRoleRequest
            {
                Role = UserRole.PhDStudent,
                EnrollmentYear = 2022
            }, token);
            Assert.True(r1.IsSuccessStatusCode);

            // Now promote to Researcher — should delete PhDStudent row
            var r2 = await Client.UpdateUserRole(userId, new UpdateUserRoleRequest
            {
                Role = UserRole.Researcher,
                Rank = "Dr",
                Specialty = "NLP",
                Office = "B3",
                PhoneNumber = "11111",
                ResearchAxisIds = new List<Guid>()
            }, token);
            Assert.True(r2.IsSuccessStatusCode,
                $"Role switch failed: {await r2.Content.ReadAsStringAsync()}");

            // PhD profile must be gone
            var phdResponse = await Client.GetPhDStudentProfileFullResponse(userId, token);
            Assert.Equal(HttpStatusCode.NotFound, phdResponse.StatusCode);

            // Researcher profile must exist
            var researcherProfile = await Client.GetResearcherProfile(userId, token);
            Assert.NotNull(researcherProfile?.Profile);
            Assert.Equal("Dr", researcherProfile!.Profile!.Rank);
        }

        // ── Validation failures ────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateUserRole_ToResearcher_MissingRequiredFields_Returns400()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateVisitorUserAsync("e2e.role.researcher.invalid@example.com", token);

            var response = await Client.UpdateUserRole(userId, new UpdateUserRoleRequest
            {
                Role = UserRole.Researcher
                // Rank, Specialty, Office, PhoneNumber intentionally missing
            }, token);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserRole_ToPhDStudent_MissingEnrollmentYear_Returns400()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateVisitorUserAsync("e2e.role.phd.invalid@example.com", token);

            var response = await Client.UpdateUserRole(userId, new UpdateUserRoleRequest
            {
                Role = UserRole.PhDStudent
                // EnrollmentYear intentionally missing (will be 0)
            }, token);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserRole_ToMasterian_MissingRequiredFields_Returns400()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateVisitorUserAsync("e2e.role.masterian.invalid@example.com", token);

            var response = await Client.UpdateUserRole(userId, new UpdateUserRoleRequest
            {
                Role = UserRole.Masterian
                // Cohort and DissertationSubject intentionally missing
            }, token);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // ── Authorization ──────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateUserRole_Unauthenticated_Returns401()
        {
            var response = await Client.UpdateUserRole(Guid.NewGuid(), new UpdateUserRoleRequest
            {
                Role = UserRole.Researcher
            }, "invalid-token");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserRole_NonAdmin_Returns403()
        {
            var adminToken = await LoginAsSuperAdmin();

            // Create a visitor user and login as that visitor
            var visitorEmail = "e2e.role.visitor.forbidden@example.com";
            await Client.AddUser(new CreateUserRequest
            {
                FirstName = "Visitor",
                LastName = "Forbidden",
                Email = visitorEmail,
                Password = "password",
                IsActive = true
            }, adminToken);

            var visitorToken = await Client.AuthenticateUser(
                new WebAPI.Models.Auth.Login.LoginRequest(visitorEmail, "password"));

            var targetUserId = await CreateVisitorUserAsync("e2e.role.target.user@example.com", adminToken);

            var response = await Client.UpdateUserRole(targetUserId, new UpdateUserRoleRequest
            {
                Role = UserRole.Researcher,
                Rank = "Prof",
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345"
            }, visitorToken?.AccessToken ?? "");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
