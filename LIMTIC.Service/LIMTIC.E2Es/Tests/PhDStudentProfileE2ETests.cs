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
    public class PhDStudentProfileE2ETests : BaseE2ETests
    {
        public PhDStudentProfileE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture)
            : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
        {
        }

        // ── Helpers ────────────────────────────────────────────────────────────────

        private async Task<Guid?> CreatePhDStudentUserAsync(string email, string token, int enrollmentYear = 2022)
        {
            var createResponse = await Client.AddUser(new CreateUserCommand
            {
                FirstName = "PhDStudent",
                LastName = "E2E",
                Email = email,
                Password = "password",
                IsActive = true
            }, token);

            Assert.NotNull(createResponse);
            var userId = createResponse.Data?.Id;

            var roleResponse = await Client.UpdateUserRole(userId.Value, new UpdateUserRoleCommand
            {
                Role = UserRole.PhDStudent,
                EnrollmentYear = enrollmentYear
            }, token);

            Assert.True(roleResponse.Success);

            return userId;
        }

        // ── GET ────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetPhDStudentProfile_ExistingProfile_Returns200WithData()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreatePhDStudentUserAsync("e2e.get.phd@example.com", token, 2021);

            var profile = await Client.GetPhDStudentProfile(userId.Value);

            Assert.NotNull(profile);
            Assert.Equal(userId, profile!.Data?.Id);
            Assert.Equal(2021, profile.Data?.EnrollmentYear);
            Assert.Equal(UserRole.PhDStudent, profile.Data?.Role);
        }

        [Fact]
        public async Task GetPhDStudentProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.GetPhDStudentProfile(Guid.NewGuid());

            Assert.False(response.Success);
        }

        // ── UPDATE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdatePhDStudentProfile_ValidData_Returns200WithUpdatedFields()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreatePhDStudentUserAsync("e2e.update.phd@example.com", token, 2022);

            var updateRequest = new UpdatePhDStudentProfileCommand
            {
                EnrollmentYear = 2023,
                ThesisSubject = "Federated Learning"
            };

            var response = await Client.UpdatePhDStudentProfile(userId.Value, updateRequest, token);

            Assert.True(response.Success);

            // Verify changes persisted
            var profile = await Client.GetPhDStudentProfile(userId.Value);
            Assert.NotNull(profile?.Data);
            Assert.Equal(2023, profile!.Data!.EnrollmentYear);
            Assert.Equal("Federated Learning", profile.Data.ThesisSubject);
        }

        [Fact]
        public async Task UpdatePhDStudentProfile_InvalidEnrollmentYear_Returns400()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreatePhDStudentUserAsync("e2e.update.phd.invalid@example.com", token);

            var updateRequest = new UpdatePhDStudentProfileCommand
            {
                EnrollmentYear = 0   // must be > 0
            };

            var response = await Client.UpdatePhDStudentProfile(userId.Value, updateRequest, token);

            Assert.False(response.Success);
        }

        [Fact]
        public async Task UpdatePhDStudentProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var updateRequest = new UpdatePhDStudentProfileCommand
            {
                EnrollmentYear = 2022
            };

            var response = await Client.UpdatePhDStudentProfile(Guid.NewGuid(), updateRequest, token);

            Assert.False(response.Success);
        }

        [Fact]
        public async Task UpdatePhDStudentProfile_Unauthenticated_Returns401()
        {
            var updateRequest = new UpdatePhDStudentProfileCommand
            {
                EnrollmentYear = 2022
            };

            var response = await Client.UpdatePhDStudentProfile(Guid.NewGuid(), updateRequest, "invalid-token");

            Assert.False(response.Success);
        }

        // ── DELETE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeletePhDStudentProfile_ExistingProfile_Returns200AndResetsRoleToVisitor()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreatePhDStudentUserAsync("e2e.delete.phd@example.com", token);

            var deleteResponse = await Client.DeletePhDStudentProfile(userId.Value, token);

            Assert.True(deleteResponse.Success);

            // Profile must be gone
            var profileResponse = await Client.GetPhDStudentProfile(userId.Value);
            Assert.False(profileResponse.Success);

            // User role must be reset to Visitor
            var user = await Client.GetUserById(userId.Value, token);
            Assert.NotNull(user);
            Assert.Equal(UserRole.Visitor, user?.Data?.Role);
        }

        [Fact]
        public async Task DeletePhDStudentProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.DeletePhDStudentProfile(Guid.NewGuid(), token);

            Assert.False(response.Success);
        }

        [Fact]
        public async Task DeletePhDStudentProfile_NonAdmin_Returns403()
        {
            var adminToken = await LoginAsSuperAdmin();

            // Create a regular visitor user and get their token
            await Client.AddUser(new CreateUserCommand
            {
                FirstName = "Visitor",
                LastName = "User",
                Email = "e2e.visitor.delete.phd@example.com",
                Password = "password",
                IsActive = true
            }, adminToken);

            var visitorToken = await Client.AuthenticateUser(
                new LoginCommand("e2e.visitor.delete.phd@example.com", "password"));

            // Create a PhD student to attempt to delete
            var phdUserId = await CreatePhDStudentUserAsync("e2e.delete.phd.target@example.com", adminToken);

            var response = await Client.DeletePhDStudentProfile(phdUserId.Value, visitorToken?.Data?.AccessToken ?? "");

            Assert.False(response.Success);
        }
    }
}
