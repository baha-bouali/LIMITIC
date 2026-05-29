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
    public class ResearcherProfileE2ETests : BaseE2ETests
    {
        public ResearcherProfileE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture)
            : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
        {
        }

        // ── Helpers ────────────────────────────────────────────────────────────────

        private async Task<Guid> CreateResearcherUserAsync(string email, string token)
        {
            var createResponse = await Client.AddUser(new CreateUserCommand
            {
                FirstName = "Researcher",
                LastName = "E2E",
                Email = email,
                Password = "password",
                IsActive = true
            }, token);

            Assert.NotNull(createResponse);
            var userId = createResponse.Data?.Id;

            var roleResponse = await Client.UpdateUserRole(userId.Value, new UpdateUserRoleCommand
            {
                Role = UserRole.Researcher,
                Rank = "Professor",
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345",
                ResearchAxisIds = new List<Guid>()
            }, token);

            Assert.True(roleResponse.Success);

            return userId.Value;
        }

        // ── GET ────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetResearcherProfile_ExistingProfile_Returns200WithData()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateResearcherUserAsync("e2e.get.researcher@example.com", token);

            var profile = await Client.GetResearcherProfile(userId);

            Assert.NotNull(profile);
            Assert.Equal(userId, profile!.Data?.Id);
            Assert.Equal("Professor", profile.Data?.Rank);
            Assert.Equal("AI", profile.Data?.Specialty);
            Assert.Equal(UserRole.Researcher, profile.Data?.Role);
        }

        [Fact]
        public async Task GetResearcherProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.GetResearcherProfile(Guid.NewGuid());

            Assert.False(response.Success);
        }

        // ── UPDATE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateResearcherProfile_ValidData_Returns200WithUpdatedFields()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateResearcherUserAsync("e2e.update.researcher@example.com", token);

            var updateRequest = new UpdateResearcherProfileCommand
            {
                Rank = "Full Professor",
                Specialty = "Deep Learning",
                Office = "Room 42",
                PhoneNumber = "99999",
                Biography = "Expert in DL",
                LinkedIn = "https://linkedin.com/test"
            };

            var response = await Client.UpdateResearcherProfile(userId, updateRequest, token);

            Assert.True(response.Success);

            // Verify changes persisted
            var profile = await Client.GetResearcherProfile(userId);
            Assert.NotNull(profile?.Data);
            Assert.Equal("Full Professor", profile!.Data!.Rank);
            Assert.Equal("Deep Learning", profile.Data.Specialty);
            Assert.Equal("Expert in DL", profile.Data.Biography);
            Assert.Equal("https://linkedin.com/test", profile.Data.LinkedIn);
        }

        [Fact]
        public async Task UpdateResearcherProfile_MissingRequiredFields_Returns400()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateResearcherUserAsync("e2e.update.researcher.invalid@example.com", token);

            var updateRequest = new UpdateResearcherProfileCommand
            {
                Rank = "",          // required
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345"
            };

            var response = await Client.UpdateResearcherProfile(userId, updateRequest, token);

            Assert.False(response.Success);
        }

        [Fact]
        public async Task UpdateResearcherProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var updateRequest = new UpdateResearcherProfileCommand
            {
                Rank = "Prof",
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345"
            };

            var response = await Client.UpdateResearcherProfile(Guid.NewGuid(), updateRequest, token);

            Assert.False(response.Success);
        }

        [Fact]
        public async Task UpdateResearcherProfile_Unauthenticated_Returns401()
        {
            var updateRequest = new UpdateResearcherProfileCommand
            {
                Rank = "Prof",
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345"
            };

            var response = await Client.UpdateResearcherProfile(Guid.NewGuid(), updateRequest, "invalid-token");

            Assert.False(response.Success);
        }

        // ── DELETE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteResearcherProfile_ExistingProfile_Returns200AndResetsRoleToVisitor()
        {
            var token = await LoginAsSuperAdmin();
            var userId = await CreateResearcherUserAsync("e2e.delete.researcher@example.com", token);

            var deleteResponse = await Client.DeleteResearcherProfile(userId, token);

            Assert.True(deleteResponse.Success);

            // Profile must be gone
            var profileResponse = await Client.GetResearcherProfile(userId);
            Assert.False(profileResponse.Success);

            // User role must be reset to Visitor
            var user = await Client.GetUserById(userId, token);
            Assert.NotNull(user);
            Assert.Equal(UserRole.Visitor, user?.Data?.Role);
        }

        [Fact]
        public async Task DeleteResearcherProfile_NonExistingProfile_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.DeleteResearcherProfile(Guid.NewGuid(), token);

            Assert.False(response.Success);
        }

        [Fact]
        public async Task DeleteResearcherProfile_NonAdmin_Returns403()
        {
            var adminToken = await LoginAsSuperAdmin();

            // Create a regular visitor user and get their token
            var visitorResponse = await Client.AddUser(new CreateUserCommand
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
                new LoginCommand("e2e.visitor.delete.researcher@example.com", "password"));

            // Create a researcher to attempt to delete
            var researcherUserId = await CreateResearcherUserAsync("e2e.delete.researcher.target@example.com", adminToken);

            var response = await Client.DeleteResearcherProfile(researcherUserId, visitorToken?.Data?.AccessToken ?? "");

            Assert.False(response.Success);
        }
    }
}
