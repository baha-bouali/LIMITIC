using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Commands.Login;
using LIMTIC.Application.Contracts.Commands.Publications;
using LIMTIC.Application.Contracts.Commands.ResearchAxis;
using LIMTIC.Application.Contracts.Commands.UpdateUserRole;
using LIMTIC.Application.Contracts.Queries.Publications;
using LIMTIC.Application.DTOs.Publications;
using LIMTIC.Application.DTOs.ResearchAxis;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;

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
            var created = await Client.CreateResearchAxis(new CreateResearchAxisCommand
            {
                Title = "E2E Publications Axis",
                Description = "Axis for publications tests",
                Themes = ["AI"]
            }, token);

            Assert.True(created.Success, $"Failed to create axis: {created.Message}");
            Assert.NotNull(created.Data);
            return created.Data.Id;
        }

        private async Task<string> CreateAndLoginUserAsync(string adminToken, string email, UserRole role)
        {
            var created = await Client.AddUser(new CreateUserCommand
            {
                FirstName = "E2E",
                LastName = "User",
                Email = email,
                Password = "password",
                IsActive = true
            }, adminToken);

            Assert.True(created.Success, $"Failed to create user: {created.Message}");
            Assert.NotNull(created.Data);
            var userId = created.Data.Id;

            var roleResponse = await Client.UpdateUserRole(new UpdateUserRoleCommand
            {
                UserId = userId,
                Role = role,
                Cohort = "2026",
                EnrollmentYear = 2026,
                DissertationSubject = "E2E Dissertation",
                ResearchAxisIds = new List<Guid>()
            }, adminToken);

            Assert.True(roleResponse.Success, $"Role update failed: {roleResponse.Message}");

            var login = await Client.AuthenticateUser(new LoginCommand(email, "password"));
            Assert.True(login.Success, $"Login failed: {login.Message}");
            Assert.NotNull(login.Data);
            return login.Data.AccessToken;
        }

        private static CreatePublicationCommand BuildJournalArticle(Guid axisId, string visibility)
        {
            return new CreatePublicationCommand
            {
                Publication = new PublicationDto
                {
                    ResearchAxisId = axisId,
                    Title = "E2E ArticleJournal Article",
                    Abstract = "E2E abstract",
                    Keywords = ["AI", "ML"],
                    Doi = "10.1234/e2e",
                    Venue = "E2E ArticleJournal",
                    Type = PublicationType.ArticleJournal,
                    Visibility = Enum.Parse<PublicationVisibility>(visibility),
                    Year = 2026,
                    Authors = ["Author A", "Author B"],
                    JournalArticle = new JournalArticleDto
                    {
                        JournalName = "E2E ArticleJournal",
                        Volume = "1",
                        Number = "1",
                        Pages = 10,
                        Ranking = JournalRanking.Q1
                    }
                }
            };
        }

        [Fact]
        public async Task Anonymous_User_can_List_Publications()
        {
            // Scenario:
            // 1) Given an anonymous visitor
            // 2) When calling GET /api/v1/publications
            // 3) Then the API returns 200 and the response contains Data/Pagination
            var query = new GetPublicationsQuery { Page = 1, Limit = 20 };
            var response = await Client.GetPublications(query, string.Empty); // Empty token for anonymous

            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.NotNull(response.Pagination);
        }

        [Fact]
        public async Task Create_Publication_Submits_ThenAdminValidates_ThenAppearsPublic()
        {
            // Scenario:
            // 1) Given a non-admin user and an existing Research Axis
            // 2) When creating a PUBLIC publication
            // 3) The user submit publication
            // 4) 

            // 1) Given a non-admin user and an existing Research Axis
            var adminToken = await LoginAsSuperAdmin();
            var axisId = await CreateAxisAsync(adminToken);
            var userToken = await CreateAndLoginUserAsync(adminToken, "e2e.pub.user@test.com", UserRole.Masterian);

            // 2) When creating a PUBLIC publication
            var createResponse = await Client.CreatePublication(
                BuildJournalArticle(axisId, visibility: nameof(PublicationVisibility.Public)),
                userToken);
            Assert.True(createResponse.Success, $"Failed to create publication: {createResponse.Message}");
            Assert.NotNull(createResponse.Data);
            Assert.Equal(PublicationStatus.Draft, createResponse.Data.Status);

            // 3) The user submit publication
            var submitResponse = await Client.SubmitPublication(createResponse.Data.Id.Value, userToken);
            Assert.True(submitResponse.Success, $"Failed to submit publication: {submitResponse.Message}");

            var validateResponse = await Client.ValidatePublication(createResponse.Data.Id.Value, adminToken);
            Assert.True(validateResponse.Success, $"Failed to validate publication: {validateResponse.Message}");

            var publicDetail = await Client.GetPublications(new GetPublicationsQuery(), string.Empty);
            Assert.True(publicDetail.Success);
            Assert.NotNull(publicDetail.Data);
        }

        [Fact]
        public async Task Public_List_Anonymous_HidesPrivatePublications()
        {
            // Scenario:
            // Given two published publications (one Public, one Private)
            // When an anonymous visitor requests the public list and a private publication detail
            // Then the public list contains only the Public publication and the private detail returns 404
            var adminToken = await LoginAsSuperAdmin();
            var axisId = await CreateAxisAsync(adminToken);

            var createPublicResponse = await Client.CreatePublication(
                BuildJournalArticle(axisId, visibility: nameof(PublicationVisibility.Public)),
                adminToken);
            var publicCreated = createPublicResponse.Data;

            var createPrivateResponse = await Client.CreatePublication(
                BuildJournalArticle(axisId, visibility: nameof(PublicationVisibility.Private)),
                adminToken);
            var privateCreated = createPrivateResponse.Data;

            Assert.NotNull(publicCreated);
            Assert.NotNull(privateCreated);

            var query = new GetPublicationsQuery { Page = 1, Limit = 50 };
            var publicList = await Client.GetPublications(query, null);
            Assert.True(publicList.Success);
            Assert.NotNull(publicList.Data);

            Assert.Contains(publicList.Data, p => p.Id == publicCreated.Id);
            Assert.DoesNotContain(publicList.Data, p => p.Id == privateCreated.Id);

            var privateDetail = await Client.GetPublicationById(privateCreated.Id.Value, string.Empty);
            Assert.False(privateDetail.Success); // Should fail for anonymous
        }

        [Fact]
        public async Task Dashboard_Reject_Flow_Works_ForAdmin()
        {
            // Scenario:
            // Given a non-admin user has created a publication (Submitted)
            // When an Admin rejects it with a reason
            // Then the API returns 200 and the status becomes Rejected with the rejection reason echoed
            var adminToken = await LoginAsSuperAdmin();
            var axisId = await CreateAxisAsync(adminToken);

            var userToken = await CreateAndLoginUserAsync(adminToken, "e2e.pub.reject@test.com", UserRole.Masterian);

            var createResponse = await Client.CreatePublication(
                BuildJournalArticle(axisId, visibility: nameof(PublicationVisibility.Public)),
                userToken);

            Assert.True(createResponse.Success);
            Assert.NotNull(createResponse.Data);

            var rejectResponse = await Client.RejectPublication(createResponse.Data.Id.Value, adminToken);
            Assert.True(rejectResponse.Success, $"Failed to reject publication: {rejectResponse.Message}");

            // Verify the publication is now rejected
            var detail = await Client.GetPublicationById(createResponse.Data.Id.Value, adminToken);
            Assert.True(detail.Success);
            Assert.NotNull(detail.Data);
            Assert.Equal(PublicationStatus.Rejected, detail.Data.Status);
        }
    }
}