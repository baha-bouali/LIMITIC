using System.Net;
using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Commands.Login;
using LIMTIC.Application.Contracts.Commands.ResearchAxis;
using LIMTIC.Application.Contracts.Commands.UpdateUserRole;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class ResearchAxisE2ETests : BaseE2ETests
    {
        public ResearchAxisE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture)
            : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
        {
        }

        // ── Helpers ────────────────────────────────────────────────────────────────

        private async Task<Guid> CreateAxisAsync(string token, string title = "AI Research", string description = "Artificial Intelligence")
        {
            var result = await Client.CreateResearchAxis(new CreateResearchAxisCommand
            {
                Title = title,
                Description = description,
                Themes = ["ML", "DL"]
            }, token);

            Assert.NotNull(result?.Data);
            return result!.Data!.Id;
        }

        private async Task<Guid> CreateResearcherAsync(string email, string token)
        {
            var createResponse = await Client.AddUser(new CreateUserCommand
            {
                FirstName = "E2E",
                LastName = "Researcher",
                Email = email,
                Password = "password",
                IsActive = true
            }, token);

            Assert.NotNull(createResponse?.Data);
            var userId = createResponse!.Data.Id;

            var roleResponse = await Client.UpdateUserRole(new UpdateUserRoleCommand
            {
                UserId = userId,
                Role = UserRole.Researcher,
                Rank = "Professor",
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345",
                ResearchAxisIds = []
            }, token);

            Assert.True(roleResponse.Success);

            return userId;
        }

        // ── GET ALL ────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllResearchAxes_Returns200WithList()
        {
            var token = await LoginAsSuperAdmin();
            await CreateAxisAsync(token, "E2E Axis A", "Description A");
            await CreateAxisAsync(token, "E2E Axis B", "Description B");

            var result = await Client.GetAllResearchAxes();

            Assert.NotNull(result);
            Assert.True(result?.Data?.Count >= 2);
        }

        // ── GET BY ID ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetResearchAxisById_Existing_Returns200WithData()
        {
            var token = await LoginAsSuperAdmin();
            var id = await CreateAxisAsync(token, "E2E Get Axis", "Some description");

            var result = await Client.GetResearchAxisById(id);

            Assert.NotNull(result?.Data);
            Assert.Equal("E2E Get Axis", result!.Data!.Title);
        }

        [Fact]
        public async Task GetResearchAxisById_NonExisting_Returns404()
        {
            var token = await LoginAsSuperAdmin();
            var response = await Client.GetResearchAxisById(Guid.NewGuid());
            Assert.False(response.Success);
        }

        // ── CREATE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateResearchAxis_ValidData_Returns200WithDto()
        {
            var token = await LoginAsSuperAdmin();

            var result = await Client.CreateResearchAxis(new CreateResearchAxisCommand
            {
                Title = "Cybersecurity E2E",
                Description = "Security research",
                Themes = ["Crypto", "Networking"]
            }, token);

            Assert.NotNull(result?.Data);
            Assert.Equal("Cybersecurity E2E", result!.Data!.Title);
            Assert.Equal(2, result.Data.Themes.Length);
        }

        [Fact]
        public async Task CreateResearchAxis_MissingTitle_Returns400()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.CreateResearchAxis(new CreateResearchAxisCommand
            {
                Title = "",
                Description = "Some description"
            }, token);

            Assert.False(response.Success);
        }

        [Fact]
        public async Task CreateResearchAxis_NonAdmin_Returns403()
        {
            var adminToken = await LoginAsSuperAdmin();
            await Client.AddUser(new CreateUserCommand
            {
                FirstName = "V", LastName = "U",
                Email = "e2e.axis.visitor@example.com",
                Password = "password", IsActive = true
            }, adminToken);

            var visitorToken = await Client.AuthenticateUser(
                new LoginCommand("e2e.axis.visitor@example.com", "password"));

            var response = await Client.CreateResearchAxis(new CreateResearchAxisCommand
            {
                Title = "Unauthorized",
                Description = "Should fail"
            }, visitorToken?.Data?.AccessToken ?? "");

            Assert.False(response.Success);
        }

        // ── UPDATE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateResearchAxis_ValidData_Returns200WithUpdatedFields()
        {
            var token = await LoginAsSuperAdmin();
            var id = await CreateAxisAsync(token, "Original Title", "Original Desc");

            var response = await Client.UpdateResearchAxis(id, new UpdateResearchAxisCommand
            {
                Title = "Updated Title",
                Description = "Updated Desc",
                Themes = ["Theme X"]
            }, token);

            Assert.True(response.Success);

            var updated = await Client.GetResearchAxisById(id);
            Assert.Equal("Updated Title", updated?.Data?.Title);
        }

        [Fact]
        public async Task UpdateResearchAxis_NonExisting_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.UpdateResearchAxis(Guid.NewGuid(), new UpdateResearchAxisCommand
            {
                Title = "Ghost",
                Description = "Ghost"
            }, token);

            Assert.False(response.Success);
        }

        // ── DELETE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteResearchAxis_Existing_Returns200()
        {
            var token = await LoginAsSuperAdmin();
            var id = await CreateAxisAsync(token, "To Delete E2E", "Will be deleted");

            var response = await Client.DeleteResearchAxis(id, token);
            Assert.True(response.Success);

            var getResponse = await Client.GetResearchAxisById(id);
            Assert.False(getResponse.Success);
        }

        [Fact]
        public async Task DeleteResearchAxis_NonExisting_Returns404()
        {
            var token = await LoginAsSuperAdmin();
            var response = await Client.DeleteResearchAxis(Guid.NewGuid(), token);
            Assert.False(response.Success);
        }

        // ── COLOR & RESPONSIBLE ────────────────────────────────────────────────────

        [Fact]
        public async Task CreateResearchAxis_WithColor_ReturnsColorInResponse()
        {
            var token = await LoginAsSuperAdmin();

            var result = await Client.CreateResearchAxis(new CreateResearchAxisCommand
            {
                Title = "E2E Color Axis",
                Description = "Has a color",
                Color = "#FF5733"
            }, token);

            Assert.NotNull(result?.Data);
            Assert.Equal("#FF5733", result!.Data!.Color);
        }

        [Fact]
        public async Task CreateResearchAxis_WithValidResponsible_PersistsResponsibleId()
        {
            var token = await LoginAsSuperAdmin();
            var researcherId = await CreateResearcherAsync("e2e.axis.responsible@example.com", token);

            var result = await Client.CreateResearchAxis(new CreateResearchAxisCommand
            {
                Title = "E2E Responsible Axis",
                Description = "Has a responsible researcher",
                ResponsibleId = researcherId
            }, token);

            Assert.NotNull(result?.Data);
            Assert.Equal(researcherId, result!.Data!.ResponsibleId);
        }

        [Fact]
        public async Task CreateResearchAxis_WithNonResearcherResponsible_Returns400()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.CreateResearchAxis(new CreateResearchAxisCommand
            {
                Title = "Bad Responsible Axis",
                Description = "Should fail",
                ResponsibleId = Guid.NewGuid()
            }, token);

            Assert.False(response.Success);
        }

        [Fact]
        public async Task UpdateResearchAxis_WithColorAndResponsible_UpdatesBoth()
        {
            var token = await LoginAsSuperAdmin();
            var researcherId = await CreateResearcherAsync("e2e.axis.update.responsible@example.com", token);
            var id = await CreateAxisAsync(token, "E2E Update Color Axis", "To be updated");

            var response = await Client.UpdateResearchAxis(id, new UpdateResearchAxisCommand
            {
                Title = "E2E Update Color Axis",
                Description = "Updated",
                Color = "#AABBCC",
                ResponsibleId = researcherId
            }, token);

            Assert.True(response.Success);

            var updated = await Client.GetResearchAxisById(id);
            Assert.Equal("#AABBCC", updated?.Data?.Color);
            Assert.Equal(researcherId, updated?.Data?.ResponsibleId);
        }

        // ── MEMBER MANAGEMENT ─────────────────────────────────────────────────────

        [Fact]
        public async Task AddMember_ValidResearcher_Returns200AndAppearsInAxis()
        {
            var token = await LoginAsSuperAdmin();
            var researcherId = await CreateResearcherAsync("e2e.addmember@example.com", token);
            var axisId = await CreateAxisAsync(token, "E2E Add Member Axis", "For member testing");

            var response = await Client.AddAxisMember(axisId, researcherId, token);
            Assert.True(response.Success);

            var axis = await Client.GetResearchAxisById(axisId);
            Assert.Contains(axis!.Data!.Members, m => m.Id == researcherId);
        }

        [Fact]
        public async Task AddMember_NonResearcher_Returns400()
        {
            var token = await LoginAsSuperAdmin();
            var axisId = await CreateAxisAsync(token, "E2E NonResearcher Member Axis", "Rejects non-researcher");

            var response = await Client.AddAxisMember(axisId, Guid.NewGuid(), token);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task AddMember_AxisNotFound_Returns400()
        {
            var token = await LoginAsSuperAdmin();
            var researcherId = await CreateResearcherAsync("e2e.addmember.noaxis@example.com", token);

            var response = await Client.AddAxisMember(Guid.NewGuid(), researcherId, token);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task RemoveMember_ExistingMember_Returns200AndRemovedFromAxis()
        {
            var token = await LoginAsSuperAdmin();
            var researcherId = await CreateResearcherAsync("e2e.removemember@example.com", token);
            var axisId = await CreateAxisAsync(token, "E2E Remove Member Axis", "For remove testing");

            await Client.AddAxisMember(axisId, researcherId, token);

            var removeResponse = await Client.RemoveAxisMember(axisId, researcherId, token);
            Assert.True(removeResponse.Success);

            var axis = await Client.GetResearchAxisById(axisId);
            Assert.DoesNotContain(axis!.Data!.Members, m => m.Id == researcherId);
        }

        [Fact]
        public async Task RemoveMember_NotAMember_Returns404()
        {
            var token = await LoginAsSuperAdmin();
            var axisId = await CreateAxisAsync(token, "E2E Remove Non-Member Axis", "For 404 testing");

            var response = await Client.RemoveAxisMember(axisId, Guid.NewGuid(), token);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task AddMember_NonAdmin_Returns403()
        {
            var adminToken = await LoginAsSuperAdmin();
            var axisId = await CreateAxisAsync(adminToken, "E2E Auth Member Axis", "Auth test");

            await Client.AddUser(new CreateUserCommand
            {
                FirstName = "V", LastName = "User",
                Email = "e2e.axis.member.visitor@example.com",
                Password = "password", IsActive = true
            }, adminToken);

            var visitorToken = await Client.AuthenticateUser(
                new LoginCommand("e2e.axis.member.visitor@example.com", "password"));

            var response = await Client.AddAxisMember(axisId,
                Guid.NewGuid(),
                visitorToken?.Data?.AccessToken ?? "");

            Assert.False(response.Success);
        }
    }
}
