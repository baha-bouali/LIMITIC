using System.Net;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;
using LIMTIC.WebAPI.Models.ResearchAxis;

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
            var result = await Client.CreateResearchAxis(new CreateResearchAxisRequest
            {
                Title = title,
                Description = description,
                Themes = ["ML", "DL"]
            }, token);

            Assert.NotNull(result?.ResearchAxis);
            return result!.ResearchAxis!.Id;
        }

        // ── GET ALL ────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllResearchAxes_Returns200WithList()
        {
            var token = await LoginAsSuperAdmin();
            await CreateAxisAsync(token, "E2E Axis A", "Description A");
            await CreateAxisAsync(token, "E2E Axis B", "Description B");

            var result = await Client.GetAllResearchAxes(token);

            Assert.NotNull(result);
            Assert.True(result!.ResearchAxes.Count >= 2);
        }

        [Fact]
        public async Task GetAllResearchAxes_Unauthenticated_Returns401()
        {
            var response = await Client.GetAllResearchAxesFullResponse("invalid-token");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── GET BY ID ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetResearchAxisById_Existing_Returns200WithData()
        {
            var token = await LoginAsSuperAdmin();
            var id = await CreateAxisAsync(token, "E2E Get Axis", "Some description");

            var result = await Client.GetResearchAxisById(id, token);

            Assert.NotNull(result?.ResearchAxis);
            Assert.Equal("E2E Get Axis", result!.ResearchAxis!.Title);
        }

        [Fact]
        public async Task GetResearchAxisById_NonExisting_Returns404()
        {
            var token = await LoginAsSuperAdmin();
            var response = await Client.GetResearchAxisByIdFullResponse(Guid.NewGuid(), token);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // ── CREATE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateResearchAxis_ValidData_Returns200WithDto()
        {
            var token = await LoginAsSuperAdmin();

            var result = await Client.CreateResearchAxis(new CreateResearchAxisRequest
            {
                Title = "Cybersecurity E2E",
                Description = "Security research",
                Themes = ["Crypto", "Networking"]
            }, token);

            Assert.NotNull(result?.ResearchAxis);
            Assert.Equal("Cybersecurity E2E", result!.ResearchAxis!.Title);
            Assert.Equal(2, result.ResearchAxis.Themes.Length);
        }

        [Fact]
        public async Task CreateResearchAxis_MissingTitle_Returns400()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.CreateResearchAxisFullResponse(new CreateResearchAxisRequest
            {
                Title = "",
                Description = "Some description"
            }, token);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateResearchAxis_NonAdmin_Returns403()
        {
            var adminToken = await LoginAsSuperAdmin();
            await Client.AddUser(new WebAPI.Models.UserManagement.CreateUser.CreateUserRequest
            {
                FirstName = "V", LastName = "U",
                Email = "e2e.axis.visitor@example.com",
                Password = "password", IsActive = true
            }, adminToken);

            var visitorToken = await Client.AuthenticateUser(
                new WebAPI.Models.Auth.Login.LoginRequest("e2e.axis.visitor@example.com", "password"));

            var response = await Client.CreateResearchAxisFullResponse(new CreateResearchAxisRequest
            {
                Title = "Unauthorized",
                Description = "Should fail"
            }, visitorToken?.AccessToken ?? "");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        // ── UPDATE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateResearchAxis_ValidData_Returns200WithUpdatedFields()
        {
            var token = await LoginAsSuperAdmin();
            var id = await CreateAxisAsync(token, "Original Title", "Original Desc");

            var response = await Client.UpdateResearchAxis(id, new UpdateResearchAxisRequest
            {
                Title = "Updated Title",
                Description = "Updated Desc",
                Themes = ["Theme X"]
            }, token);

            Assert.True(response.IsSuccessStatusCode,
                $"Update failed: {await response.Content.ReadAsStringAsync()}");

            var updated = await Client.GetResearchAxisById(id, token);
            Assert.Equal("Updated Title", updated?.ResearchAxis?.Title);
        }

        [Fact]
        public async Task UpdateResearchAxis_NonExisting_Returns404()
        {
            var token = await LoginAsSuperAdmin();

            var response = await Client.UpdateResearchAxis(Guid.NewGuid(), new UpdateResearchAxisRequest
            {
                Title = "Ghost",
                Description = "Ghost"
            }, token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // ── DELETE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteResearchAxis_Existing_Returns200()
        {
            var token = await LoginAsSuperAdmin();
            var id = await CreateAxisAsync(token, "To Delete E2E", "Will be deleted");

            var response = await Client.DeleteResearchAxis(id, token);
            Assert.True(response.IsSuccessStatusCode,
                $"Delete failed: {await response.Content.ReadAsStringAsync()}");

            var getResponse = await Client.GetResearchAxisByIdFullResponse(id, token);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteResearchAxis_NonExisting_Returns404()
        {
            var token = await LoginAsSuperAdmin();
            var response = await Client.DeleteResearchAxis(Guid.NewGuid(), token);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
