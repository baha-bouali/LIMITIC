using System.Net;
using System.Net.Http.Json;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;
using LIMTIC.WebAPI.Models.Auth.Login;
using LIMTIC.WebAPI.Models.Publications.Dashboard.CreatePublication;
using LIMTIC.WebAPI.Models.Publications.Dashboard.Pdf;
using LIMTIC.WebAPI.Models.Publications.Dashboard.Reject;
using LIMTIC.WebAPI.Models.Publications.Dashboard.Status;
using LIMTIC.WebAPI.Models.ResearchAxis;
using LIMTIC.WebAPI.Models.UserManagement.CreateUser;
using LIMTIC.WebAPI.Models.UserManagement.UpdateUserRole;
using Xunit;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class PublicationsE2ETests : BaseE2ETests
    {
        public PublicationsE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture)
            : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
        {
        }

        private async Task<Guid> CreateAxisAsync(string token)
        {
            var created = await Client.CreateResearchAxis(new CreateResearchAxisRequest
            {
                Title = "E2E Publications Axis",
                Description = "Axis for publications tests",
                Themes = ["AI"]
            }, token);

            Assert.NotNull(created?.ResearchAxis);
            return created!.ResearchAxis!.Id;
        }

        private async Task<string> CreateAndLoginUserAsync(string adminToken, string email, UserRole role)
        {
            var created = await Client.AddUser(new CreateUserRequest
            {
                FirstName = "E2E",
                LastName = "User",
                Email = email,
                Password = "password",
                IsActive = true
            }, adminToken);

            Assert.NotNull(created?.User);
            var userId = created!.User.Id;

            var roleResponse = await Client.UpdateUserRole(userId, new UpdateUserRoleRequest
            {
                Role = role,
                Cohort = "2026",
                EnrollmentYear = 2026,
                DissertationSubject = "E2E Dissertation",
                ResearchAxisIds = []
            }, adminToken);

            Assert.True(roleResponse.IsSuccessStatusCode,
                $"Role update failed: {await roleResponse.Content.ReadAsStringAsync()}");

            var login = await Client.AuthenticateUser(new LoginRequest(email, "password"));
            Assert.NotNull(login?.AccessToken);
            return login!.AccessToken!;
        }

        private static CreateDashboardPublicationRequest BuildJournalArticle(Guid axisId, string visibility)
        {
            return new CreateDashboardPublicationRequest
            {
                ResearchAxisId = axisId,
                Title = "E2E Journal Article",
                Abstract = "E2E abstract",
                Keywords = ["AI", "ML"],
                Doi = "10.1234/e2e",
                Venue = "E2E Journal",
                Type = nameof(PublicationType.ArticleJournal),
                Visibility = visibility,
                Year = 2026,
                Authors = ["Author A", "Author B"],
                JournalArticle = new CreateJournalArticleRequestItem
                {
                    JournalName = "E2E Journal",
                    Volume = "1",
                    Number = "1",
                    Pages = "1-10",
                    Ranking = nameof(JournalRanking.Q1)
                }
            };
        }

        [Fact]
        public async Task Public_List_Anonymous_Returns200_WithShape()
        {
            // Scenario:
            // Given an anonymous visitor
            // When calling GET /api/v1/public/publications
            // Then the API returns 200 and the response contains Data/Stats/Pagination
            var response = await Client.GetPublicPublicationsFullResponse();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<LIMTIC.WebAPI.Models.Publications.Public.GetPublications.PublicationsListResponse>();
            Assert.NotNull(body);
            Assert.NotNull(body!.Data);
            Assert.NotNull(body.Stats);
            Assert.NotNull(body.Pagination);
        }

        [Fact]
        public async Task Dashboard_Create_ByAdmin_Publishes_AndAppearsInPublicList()
        {
            // Scenario:
            // Given an Admin user and an existing Research Axis
            // When creating a PUBLIC publication from the dashboard
            // Then it is created as Published and is visible in public list and public detail
            var adminToken = await LoginAsSuperAdmin();
            var axisId = await CreateAxisAsync(adminToken!);

            var createHttp = await Client.CreateDashboardPublication(
                BuildJournalArticle(axisId, visibility: nameof(PublicationVisibility.Public)),
                adminToken!);

            Assert.Equal(HttpStatusCode.Created, createHttp.StatusCode);

            var created = await createHttp.Content.ReadFromJsonAsync<CreateDashboardPublicationResponse>();
            Assert.NotNull(created);
            Assert.Equal(nameof(PublicationStatus.Published), created!.Publication.Status);

            var publicList = await Client.GetPublicPublications(page: 1, limit: 20);
            Assert.NotNull(publicList);
            Assert.Contains(publicList!.Data, p => p.Id == created.Publication.Id);

            var detail = await Client.GetPublicPublicationById(created.Publication.Id);
            Assert.NotNull(detail);
            Assert.Equal(created.Publication.Id, detail!.Id);
            Assert.Equal(nameof(PublicationStatus.Published), detail.Status);
        }

        [Fact]
        public async Task Dashboard_Create_ByNonAdmin_Submits_ThenAdminValidates_ThenAppearsPublic()
        {
            // Scenario:
            // Given a non-admin user and an existing Research Axis
            // When creating a PUBLIC publication from the dashboard
            // Then it is created as Submitted and only becomes visible publicly after Admin validation
            var adminToken = await LoginAsSuperAdmin();
            var axisId = await CreateAxisAsync(adminToken!);

            var userToken = await CreateAndLoginUserAsync(adminToken!, "e2e.pub.user@test.com", UserRole.Masterian);

            var createHttp = await Client.CreateDashboardPublication(
                BuildJournalArticle(axisId, visibility: nameof(PublicationVisibility.Public)),
                userToken);

            Assert.Equal(HttpStatusCode.Created, createHttp.StatusCode);
            var created = await createHttp.Content.ReadFromJsonAsync<CreateDashboardPublicationResponse>();
            Assert.NotNull(created);
            Assert.Equal(nameof(PublicationStatus.Submitted), created!.Publication.Status);

            var validateHttp = await Client.ValidateDashboardPublication(created.Publication.Id, adminToken!);
            Assert.Equal(HttpStatusCode.OK, validateHttp.StatusCode);
            var validated = await validateHttp.Content.ReadFromJsonAsync<PublicationValidatedResponse>();
            Assert.NotNull(validated);
            Assert.Equal(nameof(PublicationStatus.Published), validated!.Status);

            var publicDetail = await Client.GetPublicPublicationById(created.Publication.Id);
            Assert.NotNull(publicDetail);
            Assert.Equal(nameof(PublicationStatus.Published), publicDetail!.Status);
        }

        [Fact]
        public async Task Public_List_Anonymous_HidesPrivatePublications()
        {
            // Scenario:
            // Given two published publications (one Public, one Private)
            // When an anonymous visitor requests the public list and a private publication detail
            // Then the public list contains only the Public publication and the private detail returns 404
            var adminToken = await LoginAsSuperAdmin();
            var axisId = await CreateAxisAsync(adminToken!);

            var createPublicHttp = await Client.CreateDashboardPublication(
                BuildJournalArticle(axisId, visibility: nameof(PublicationVisibility.Public)),
                adminToken!);
            var publicCreated = await createPublicHttp.Content.ReadFromJsonAsync<CreateDashboardPublicationResponse>();

            var createPrivateHttp = await Client.CreateDashboardPublication(
                BuildJournalArticle(axisId, visibility: nameof(PublicationVisibility.Private)),
                adminToken!);
            var privateCreated = await createPrivateHttp.Content.ReadFromJsonAsync<CreateDashboardPublicationResponse>();

            Assert.NotNull(publicCreated);
            Assert.NotNull(privateCreated);

            var publicList = await Client.GetPublicPublications(page: 1, limit: 50);
            Assert.NotNull(publicList);

            Assert.Contains(publicList!.Data, p => p.Id == publicCreated!.Publication.Id);
            Assert.DoesNotContain(publicList.Data, p => p.Id == privateCreated!.Publication.Id);

            var privateDetailHttp = await Client.GetAsync($"api/v1/public/publications/{privateCreated.Publication.Id}");
            Assert.Equal(HttpStatusCode.NotFound, privateDetailHttp.StatusCode);
        }

        [Fact]
        public async Task Dashboard_Reject_Flow_Works_ForAdmin()
        {
            // Scenario:
            // Given a non-admin user has created a publication (Submitted)
            // When an Admin rejects it with a reason
            // Then the API returns 200 and the status becomes Rejected with the rejection reason echoed
            var adminToken = await LoginAsSuperAdmin();
            var axisId = await CreateAxisAsync(adminToken!);

            var userToken = await CreateAndLoginUserAsync(adminToken!, "e2e.pub.reject@test.com", UserRole.Masterian);

            var createHttp = await Client.CreateDashboardPublication(
                BuildJournalArticle(axisId, visibility: nameof(PublicationVisibility.Public)),
                userToken);

            var created = await createHttp.Content.ReadFromJsonAsync<CreateDashboardPublicationResponse>();
            Assert.NotNull(created);

            var rejectHttp = await Client.RejectDashboardPublication(created!.Publication.Id,
                new RejectPublicationRequest { Reason = "Incomplete metadata" }, adminToken!);

            Assert.Equal(HttpStatusCode.OK, rejectHttp.StatusCode);
            var rejected = await rejectHttp.Content.ReadFromJsonAsync<PublicationRejectedResponse>();
            Assert.NotNull(rejected);
            Assert.Equal(nameof(PublicationStatus.Rejected), rejected!.Status);
            Assert.Equal("Incomplete metadata", rejected.RejectionReason);
        }

        [Fact]
        public async Task Dashboard_Pdf_AddAndRemove_Works_ForOwner()
        {
            // Scenario:
            // Given a user created a publication from the dashboard
            // When the owner adds a PDF URL then removes it
            // Then the add/remove endpoints return 200 and the publication detail reflects the PDF change
            var adminToken = await LoginAsSuperAdmin();
            var axisId = await CreateAxisAsync(adminToken!);

            var userToken = await CreateAndLoginUserAsync(adminToken!, "e2e.pub.pdf@test.com", UserRole.Masterian);

            var createHttp = await Client.CreateDashboardPublication(
                BuildJournalArticle(axisId, visibility: nameof(PublicationVisibility.Public)),
                userToken);
            var created = await createHttp.Content.ReadFromJsonAsync<CreateDashboardPublicationResponse>();
            Assert.NotNull(created);

            var pdfUrl = "https://storage.example.com/e2e.pdf";
            var addHttp = await Client.AddDashboardPublicationPdf(created!.Publication.Id,
                new PdfRequest { PdfUrl = pdfUrl }, userToken);
            Assert.Equal(HttpStatusCode.OK, addHttp.StatusCode);

            var detail = await Client.GetDashboardPublicationById(created.Publication.Id, userToken);
            Assert.NotNull(detail);
            Assert.Equal(pdfUrl, detail!.PdfUrl);

            var removeHttp = await Client.RemoveDashboardPublicationPdf(created.Publication.Id,
                new PdfRequest { PdfUrl = pdfUrl }, userToken);
            Assert.Equal(HttpStatusCode.OK, removeHttp.StatusCode);
        }
    }
}
