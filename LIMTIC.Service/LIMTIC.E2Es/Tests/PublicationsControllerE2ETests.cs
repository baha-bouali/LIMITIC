//using System.Net;
//using System.Net.Http.Json;
//using LIMTIC.Domain.Enums;
//using LIMTIC.E2Es.Base;
//using LIMTIC.E2Es.Extensions;
//using LIMTIC.E2Es.MailFixture;
//using LIMTIC.WebAPI.Models.Auth.Login;
//using LIMTIC.WebAPI.Models.UserManagement.CreateUser;
//using Xunit;

//namespace LIMTIC.E2Es.Tests
//{
//    // ══════════════════════════════════════════════════════════════════════════
//    // Shared payload builders
//    // Enum string values match the domain exactly:
//    //   PublicationType  : ArticleJournal | ConferenceInternational |
//    //                      ConferenceNational | ChapterBook | TechnicalReport
//    //   PublicationStatus: Draft | Submitted | Published | Rejected
//    //   PublicationVisibility: Public | Private
//    // ══════════════════════════════════════════════════════════════════════════

//    file static class PublicationPayloads
//    {
//        public static object JournalArticle(string title = "Test Journal Article") => new
//        {
//            title = title,
//            year = 2024,
//            authors = new[] { "Author A", "Author B" },
//            venue = "Test Journal",
//            doi = "10.1234/test",
//            type = nameof(PublicationType.ArticleJournal),
//            status = nameof(PublicationStatus.Draft),
//            visibility = nameof(PublicationVisibility.Public),
//            journalArticle = new
//            {
//                journalName = "Test Journal",
//                volume = "12",
//                number = "3",
//                pages = "100-110",
//                ranking = "Q1"
//            }
//        };

//        public static object ConferencePaper(string title = "Test Conference Paper") => new
//        {
//            title = title,
//            year = 2023,
//            authors = new[] { "Author C" },
//            venue = "Test Conference",
//            type = nameof(PublicationType.ConferenceInternational),
//            status = nameof(PublicationStatus.Draft),
//            visibility = nameof(PublicationVisibility.Public),
//            internationalConference = new
//            {
//                location = "Paris, France",
//                pages = "55-60"
//            }
//        };

//        public static object PrivateJournalArticle(string title = "Private Article") => new
//        {
//            title = title,
//            year = 2024,
//            authors = new[] { "Author Z" },
//            venue = "Private Journal",
//            type = nameof(PublicationType.ArticleJournal),
//            status = nameof(PublicationStatus.Draft),
//            visibility = nameof(PublicationVisibility.Private),
//            journalArticle = new
//            {
//                journalName = "Private Journal",
//                ranking = "Q2"
//            }
//        };
//    }

//    // ══════════════════════════════════════════════════════════════════════════
//    // 1. Public Publications Endpoints
//    //    GET /api/v1/publications
//    //    GET /api/v1/publications/recent
//    //    GET /api/v1/publications/{id}
//    // ══════════════════════════════════════════════════════════════════════════

//    [Collection("E2E collection")]
//    public class PublicationsControllerE2ETests : BaseE2ETests
//    {
//        public PublicationsControllerE2ETests(PostgresFixture db, MailHogFixture mail)
//            : base(db, mail) { }

//        // ── GET /publications ──────────────────────────────────────────────────

//        [Fact]
//        public async Task GetPublications_Anonymous_Returns200()
//        {
//            var response = await Client.GetAsync("/api/v1/publications");
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task GetPublications_ResponseShape_HasDataStatsPagination()
//        {
//            var response = await Client.GetAsync("/api/v1/publications");
//            var body = await response.Content.ReadFromJsonAsync<PublicationsListResponse>();

//            Assert.NotNull(body);
//            Assert.NotNull(body!.Data);
//            Assert.NotNull(body.Stats);
//            Assert.NotNull(body.Pagination);
//        }

//        [Fact]
//        public async Task GetPublications_DefaultPagination_IsPage1Limit10()
//        {
//            var body = await Client.GetAsync("/api/v1/publications")
//                                   .ReadAs<PublicationsListResponse>();

//            Assert.Equal(1, body!.Pagination.Page);
//            Assert.Equal(10, body.Pagination.Limit);
//        }

//        [Fact]
//        public async Task GetPublications_CustomPageAndLimit_ReflectedInPagination()
//        {
//            var body = await Client.GetAsync("/api/v1/publications?page=2&limit=5")
//                                   .ReadAs<PublicationsListResponse>();

//            Assert.Equal(2, body!.Pagination.Page);
//            Assert.Equal(5, body.Pagination.Limit);
//        }

//        [Fact]
//        public async Task GetPublications_WithTypeFilter_ArticleJournal_Returns200()
//        {
//            var response = await Client.GetAsync(
//                $"/api/v1/publications?type={nameof(PublicationType.ArticleJournal)}");
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task GetPublications_WithTypeFilter_ConferenceInternational_Returns200()
//        {
//            var response = await Client.GetAsync(
//                $"/api/v1/publications?type={nameof(PublicationType.ConferenceInternational)}");
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task GetPublications_WithYearFilter_Returns200()
//        {
//            var response = await Client.GetAsync("/api/v1/publications?year=2024");
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task GetPublications_WithSearchFilter_Returns200()
//        {
//            var response = await Client.GetAsync("/api/v1/publications?search=machine+learning");
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task GetPublications_WithAxeIdFilter_Returns200()
//        {
//            var response = await Client.GetAsync($"/api/v1/publications?axeId={Guid.NewGuid()}");
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task GetPublications_StatsBlock_NonNegativeValues()
//        {
//            var body = await Client.GetAsync("/api/v1/publications")
//                                   .ReadAs<PublicationsListResponse>();

//            Assert.True(body!.Stats.Total >= 0);
//            Assert.True(body.Stats.Journals >= 0);
//            Assert.True(body.Stats.Conferences >= 0);
//        }

//        // ── GET /publications/recent ───────────────────────────────────────────

//        [Fact]
//        public async Task GetRecentPublications_Anonymous_Returns200()
//        {
//            var response = await Client.GetAsync("/api/v1/publications/recent");
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task GetRecentPublications_DefaultLimit_ReturnsAtMost3Items()
//        {
//            var items = await Client.GetAsync("/api/v1/publications/recent")
//                                    .ReadAs<List<PublicationSummary>>();

//            Assert.NotNull(items);
//            Assert.True(items!.Count <= 3);
//        }

//        [Fact]
//        public async Task GetRecentPublications_CustomLimit_ReturnsAtMostThatMany()
//        {
//            var items = await Client.GetAsync("/api/v1/publications/recent?limit=6")
//                                    .ReadAs<List<PublicationSummary>>();

//            Assert.True(items!.Count <= 6);
//        }

//        // ── GET /publications/{id} ─────────────────────────────────────────────

//        [Fact]
//        public async Task GetPublicationById_RandomGuid_Returns404()
//        {
//            var response = await Client.GetAsync($"/api/v1/publications/{Guid.NewGuid()}");
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        [Fact]
//        public async Task GetPublicationById_PublicPublishedPublication_Returns200WithCorrectId()
//        {
//            var adminToken = await LoginAsSuperAdmin();
//            var id = await SeedPublishedPublicPublication(adminToken, nameof(PublicationVisibility.Public));

//            var response = await Client.GetAsync($"/api/v1/publications/{id}");

//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//            var detail = await response.Content.ReadFromJsonAsync<PublicationDetail>();
//            Assert.Equal(id, detail!.Id);
//        }

//        [Fact]
//        public async Task GetPublicationById_PrivatePublishedPublication_Returns404ForAnonymous()
//        {
//            var adminToken = await LoginAsSuperAdmin();
//            var id = await SeedPublishedPublicPublication(adminToken, nameof(PublicationVisibility.Private));

//            var response = await Client.GetAsync($"/api/v1/publications/{id}");
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        // ─── Seed helper ──────────────────────────────────────────────────────

//        private async Task<Guid> SeedPublishedPublicPublication(string? adminToken, string visibility)
//        {
//            var payload = visibility == nameof(PublicationVisibility.Private)
//                ? PublicationPayloads.PrivateJournalArticle()
//                : PublicationPayloads.JournalArticle();

//            var create = await Client.PostAuthJsonAsync(
//                "/api/v1/dashboard/admin/publications", payload, adminToken);
//            create.EnsureSuccessStatusCode();
//            var created = await create.Content.ReadFromJsonAsync<PublicationSummary>();

//            await Client.PostAuthAsync(
//                $"/api/v1/dashboard/admin/publications/{created!.Id}/publish", adminToken);

//            return created.Id;
//        }
//    }

//    // ══════════════════════════════════════════════════════════════════════════
//    // 2. Researcher Dashboard Publications
//    //    Route: /api/v1/dashboard/researcher/publications
//    // ══════════════════════════════════════════════════════════════════════════

//    [Collection("E2E collection")]
//    public class ResearcherPublicationsE2ETests : BaseE2ETests
//    {
//        private const string Route = "/api/v1/dashboard/researcher/publications";

//        public ResearcherPublicationsE2ETests(PostgresFixture db, MailHogFixture mail)
//            : base(db, mail) { }

//        // ── Auth guards ────────────────────────────────────────────────────────

//        [Fact]
//        public async Task Get_Unauthenticated_Returns401()
//        {
//            var response = await Client.GetAsync(Route);
//            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_AsSuperAdmin_Returns403()
//        {
//            var token = await LoginAsSuperAdmin();
//            var response = await Client.GetAuthAsync(Route, token);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        [Fact]
//        public async Task Post_Unauthenticated_Returns401()
//        {
//            var response = await Client.PostAsJsonAsync(Route, PublicationPayloads.JournalArticle());
//            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//        }

//        // ── GET ────────────────────────────────────────────────────────────────

//        [Fact]
//        public async Task Get_AsResearcher_Returns200WithPaginationShape()
//        {
//            var token = await LoginAsNewUser(UserRole.Researcher, "res_get@test.com");
//            var body = await Client.GetAuthAsync(Route, token)
//                                    .ReadAs<PublicationsListResponse>();

//            Assert.NotNull(body?.Pagination);
//            Assert.NotNull(body?.Data);
//        }

//        [Fact]
//        public async Task Get_WithAllFilters_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.Researcher, "res_filter@test.com");
//            var response = await Client.GetAuthAsync(
//                $"{Route}?search=test&status={nameof(PublicationStatus.Draft)}" +
//                $"&type={nameof(PublicationType.ArticleJournal)}&year=2024&page=1&limit=5",
//                token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_OnlyReturnsOwnPublications()
//        {
//            var token1 = await LoginAsNewUser(UserRole.Researcher, "res_own1@test.com");
//            var token2 = await LoginAsNewUser(UserRole.Researcher, "res_own2@test.com");

//            await Client.PostAuthJsonAsync(Route, PublicationPayloads.JournalArticle("R1 Exclusive"), token1);

//            var body = await Client.GetAuthAsync(Route, token2)
//                                   .ReadAs<PublicationsListResponse>();

//            Assert.DoesNotContain(body!.Data, p => p.Title == "R1 Exclusive");
//        }

//        // ── POST ───────────────────────────────────────────────────────────────

//        [Fact]
//        public async Task Post_ValidPayload_Returns201WithSubmittedStatus()
//        {
//            var token = await LoginAsNewUser(UserRole.Researcher, "res_post@test.com");
//            var response = await Client.PostAuthJsonAsync(
//                Route, PublicationPayloads.JournalArticle(), token);

//            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

//            var body = await response.Content.ReadFromJsonAsync<CreatePublicationResponse>();
//            Assert.Equal(nameof(PublicationStatus.Submitted), body!.Publication.Status);
//        }

//        // ── PUT ────────────────────────────────────────────────────────────────

//        [Fact]
//        public async Task Put_NonExistentId_Returns404()
//        {
//            var token = await LoginAsNewUser(UserRole.Researcher, "res_put404@test.com");
//            var response = await Client.PutAuthJsonAsync(
//                $"{Route}/{Guid.NewGuid()}", PublicationPayloads.JournalArticle(), token);
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        [Fact]
//        public async Task Put_SubmittedPublication_Returns403()
//        {
//            var token = await LoginAsNewUser(UserRole.Researcher, "res_putsub@test.com");
//            var id = await CreateSubmittedPublication(Route, token);

//            var response = await Client.PutAuthJsonAsync(
//                $"{Route}/{id}", PublicationPayloads.JournalArticle("Updated"), token);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        [Fact]
//        public async Task Put_OtherOwnersPublication_Returns403()
//        {
//            var token1 = await LoginAsNewUser(UserRole.Researcher, "res_putowner1@test.com");
//            var token2 = await LoginAsNewUser(UserRole.Researcher, "res_putowner2@test.com");
//            var id = await CreateSubmittedPublication(Route, token1);

//            var response = await Client.PutAuthJsonAsync(
//                $"{Route}/{id}", PublicationPayloads.JournalArticle("Stolen"), token2);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        // ── DELETE ─────────────────────────────────────────────────────────────

//        [Fact]
//        public async Task Delete_NonExistentId_Returns404()
//        {
//            var token = await LoginAsNewUser(UserRole.Researcher, "res_del404@test.com");
//            var response = await Client.DeleteAuthAsync($"{Route}/{Guid.NewGuid()}", token);
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        [Fact]
//        public async Task Delete_SubmittedPublication_Returns403()
//        {
//            var token = await LoginAsNewUser(UserRole.Researcher, "res_delsub@test.com");
//            var id = await CreateSubmittedPublication(Route, token);

//            var response = await Client.DeleteAuthAsync($"{Route}/{id}", token);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        [Fact]
//        public async Task Delete_OtherOwnersPublication_Returns403()
//        {
//            var token1 = await LoginAsNewUser(UserRole.Researcher, "res_delown1@test.com");
//            var token2 = await LoginAsNewUser(UserRole.Researcher, "res_delown2@test.com");
//            var id = await CreateSubmittedPublication(Route, token1);

//            var response = await Client.DeleteAuthAsync($"{Route}/{id}", token2);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        // ── Submit ─────────────────────────────────────────────────────────────

//        [Fact]
//        public async Task Submit_NonExistentId_Returns404()
//        {
//            var token = await LoginAsNewUser(UserRole.Researcher, "res_sub404@test.com");
//            var response = await Client.PostAuthAsync($"{Route}/{Guid.NewGuid()}/submit", token);
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        [Fact]
//        public async Task Submit_OtherOwnersPublication_Returns403()
//        {
//            var token1 = await LoginAsNewUser(UserRole.Researcher, "res_subow1@test.com");
//            var token2 = await LoginAsNewUser(UserRole.Researcher, "res_subow2@test.com");
//            var id = await CreateSubmittedPublication(Route, token1);

//            var response = await Client.PostAuthAsync($"{Route}/{id}/submit", token2);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        // ── PDF ────────────────────────────────────────────────────────────────

//        [Fact]
//        public async Task AddPdf_NonExistentPublication_Returns404()
//        {
//            var token = await LoginAsNewUser(UserRole.Researcher, "res_pdf404@test.com");
//            var response = await Client.PostAuthJsonAsync(
//                $"{Route}/{Guid.NewGuid()}/pdf",
//                new { pdfUrl = "https://example.com/paper.pdf" }, token);
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        [Fact]
//        public async Task AddPdf_OwnPublication_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.Researcher, "res_pdfok@test.com");
//            var id = await CreateSubmittedPublication(Route, token);

//            var response = await Client.PostAuthJsonAsync(
//                $"{Route}/{id}/pdf",
//                new { pdfUrl = "https://example.com/paper.pdf" }, token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task AddPdf_OtherOwnersPublication_Returns403()
//        {
//            var token1 = await LoginAsNewUser(UserRole.Researcher, "res_pdfow1@test.com");
//            var token2 = await LoginAsNewUser(UserRole.Researcher, "res_pdfow2@test.com");
//            var id = await CreateSubmittedPublication(Route, token1);

//            var response = await Client.PostAuthJsonAsync(
//                $"{Route}/{id}/pdf",
//                new { pdfUrl = "https://example.com/paper.pdf" }, token2);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        [Fact]
//        public async Task RemovePdf_NonExistentPublication_Returns404()
//        {
//            var token = await LoginAsNewUser(UserRole.Researcher, "res_rmpdf404@test.com");
//            var response = await Client.DeleteAuthJsonAsync(
//                $"{Route}/{Guid.NewGuid()}/pdf",
//                new { pdfUrl = "https://example.com/paper.pdf" }, token);
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        [Fact]
//        public async Task RemovePdf_OtherOwnersPublication_Returns403()
//        {
//            var token1 = await LoginAsNewUser(UserRole.Researcher, "res_rmpow1@test.com");
//            var token2 = await LoginAsNewUser(UserRole.Researcher, "res_rmpow2@test.com");
//            var id = await CreateSubmittedPublication(Route, token1);

//            var response = await Client.DeleteAuthJsonAsync(
//                $"{Route}/{id}/pdf",
//                new { pdfUrl = "https://example.com/paper.pdf" }, token2);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        // ─── Helpers ──────────────────────────────────────────────────────────

//        private async Task<Guid> CreateSubmittedPublication(string route, string? token)
//        {
//            var response = await Client.PostAuthJsonAsync(
//                route, PublicationPayloads.JournalArticle(), token);
//            response.EnsureSuccessStatusCode();
//            var body = await response.Content.ReadFromJsonAsync<CreatePublicationResponse>();
//            return body!.Publication.Id;
//        }

//        private async Task<string?> LoginAsNewUser(UserRole role, string email)
//        {
//            var adminToken = await LoginAsSuperAdmin();
//            // AddUser(CreateUserRequest, string?) → new v1 admin overload
//            var addResponse = await Client.AddUser(new CreateUserRequest
//            {
//                FirstName = role.ToString(),
//                LastName = "User",
//                Email = email,
//                Password = "TestPass1!",
//                Role = role,
//                IsActive = true
//            }, (string?)adminToken);
//            addResponse.EnsureSuccessStatusCode();
//            // AuthenticateUser(LoginRequest, string) → new v1 overload
//            return (await Client.AuthenticateUser(new LoginRequest(email, "TestPass1!"), "v1"))?.AccessToken;
//        }
//    }

//    // ══════════════════════════════════════════════════════════════════════════
//    // 3. PhD Student — Own Publications
//    //    Route: /api/v1/dashboard/phd-student/publications
//    // ══════════════════════════════════════════════════════════════════════════

//    [Collection("E2E collection")]
//    public class PhDStudentPublicationsE2ETests : BaseE2ETests
//    {
//        private const string Route = "/api/v1/dashboard/phd-student/publications";

//        public PhDStudentPublicationsE2ETests(PostgresFixture db, MailHogFixture mail)
//            : base(db, mail) { }

//        // ── Auth guards ────────────────────────────────────────────────────────

//        [Fact]
//        public async Task Get_Unauthenticated_Returns401()
//        {
//            var response = await Client.GetAsync(Route);
//            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_AsResearcher_Returns403()
//        {
//            var token = await LoginAsNewUser(UserRole.Researcher, "phd_guard_res@test.com");
//            var response = await Client.GetAuthAsync(Route, token);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        [Fact]
//        public async Task Post_Unauthenticated_Returns401()
//        {
//            var response = await Client.PostAsJsonAsync(Route, PublicationPayloads.ConferencePaper());
//            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//        }

//        // ── GET ────────────────────────────────────────────────────────────────

//        [Fact]
//        public async Task Get_AsPhDStudent_Returns200WithPaginationShape()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_get@test.com");
//            var body = await Client.GetAuthAsync(Route, token)
//                                    .ReadAs<PublicationsListResponse>();

//            Assert.NotNull(body?.Pagination);
//            Assert.NotNull(body?.Data);
//        }

//        [Fact]
//        public async Task Get_WithStatusFilter_Submitted_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_filtersub@test.com");
//            var response = await Client.GetAuthAsync(
//                $"{Route}?status={nameof(PublicationStatus.Submitted)}", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_WithTypeFilter_ConferenceNational_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_filtercn@test.com");
//            var response = await Client.GetAuthAsync(
//                $"{Route}?type={nameof(PublicationType.ConferenceNational)}", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_OnlyReturnsOwnPublications()
//        {
//            var token1 = await LoginAsNewUser(UserRole.PhDStudent, "phd_iso1@test.com");
//            var token2 = await LoginAsNewUser(UserRole.PhDStudent, "phd_iso2@test.com");

//            await Client.PostAuthJsonAsync(
//                Route, PublicationPayloads.ConferencePaper("PhD1 Exclusive"), token1);

//            var body = await Client.GetAuthAsync(Route, token2)
//                                   .ReadAs<PublicationsListResponse>();
//            Assert.DoesNotContain(body!.Data, p => p.Title == "PhD1 Exclusive");
//        }

//        // ── POST ───────────────────────────────────────────────────────────────

//        [Fact]
//        public async Task Post_ValidPayload_Returns201WithSubmittedStatus()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_post@test.com");
//            var response = await Client.PostAuthJsonAsync(
//                Route, PublicationPayloads.ConferencePaper(), token);

//            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
//            var body = await response.Content.ReadFromJsonAsync<CreatePublicationResponse>();
//            Assert.Equal(nameof(PublicationStatus.Submitted), body!.Publication.Status);
//        }

//        // ── PUT ────────────────────────────────────────────────────────────────

//        [Fact]
//        public async Task Put_NonExistentId_Returns404()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_put404@test.com");
//            var response = await Client.PutAuthJsonAsync(
//                $"{Route}/{Guid.NewGuid()}", PublicationPayloads.ConferencePaper(), token);
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        [Fact]
//        public async Task Put_SubmittedPublication_Returns403()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_putsub@test.com");
//            var id = await CreateSubmittedPublication(token);

//            var response = await Client.PutAuthJsonAsync(
//                $"{Route}/{id}", PublicationPayloads.ConferencePaper("Updated"), token);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        [Fact]
//        public async Task Put_OtherOwnersPublication_Returns403()
//        {
//            var token1 = await LoginAsNewUser(UserRole.PhDStudent, "phd_putow1@test.com");
//            var token2 = await LoginAsNewUser(UserRole.PhDStudent, "phd_putow2@test.com");
//            var id = await CreateSubmittedPublication(token1);

//            var response = await Client.PutAuthJsonAsync(
//                $"{Route}/{id}", PublicationPayloads.ConferencePaper("Stolen"), token2);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        // ── DELETE ─────────────────────────────────────────────────────────────

//        [Fact]
//        public async Task Delete_NonExistentId_Returns404()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_del404@test.com");
//            var response = await Client.DeleteAuthAsync($"{Route}/{Guid.NewGuid()}", token);
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        [Fact]
//        public async Task Delete_SubmittedPublication_Returns403()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_delsub@test.com");
//            var id = await CreateSubmittedPublication(token);

//            var response = await Client.DeleteAuthAsync($"{Route}/{id}", token);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        [Fact]
//        public async Task Delete_OtherOwnersPublication_Returns403()
//        {
//            var token1 = await LoginAsNewUser(UserRole.PhDStudent, "phd_delow1@test.com");
//            var token2 = await LoginAsNewUser(UserRole.PhDStudent, "phd_delow2@test.com");
//            var id = await CreateSubmittedPublication(token1);

//            var response = await Client.DeleteAuthAsync($"{Route}/{id}", token2);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        // ── Submit ─────────────────────────────────────────────────────────────

//        [Fact]
//        public async Task Submit_NonExistentId_Returns404()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_sub404@test.com");
//            var response = await Client.PostAuthAsync($"{Route}/{Guid.NewGuid()}/submit", token);
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        [Fact]
//        public async Task Submit_OtherOwnersPublication_Returns403()
//        {
//            var token1 = await LoginAsNewUser(UserRole.PhDStudent, "phd_subow1@test.com");
//            var token2 = await LoginAsNewUser(UserRole.PhDStudent, "phd_subow2@test.com");
//            var id = await CreateSubmittedPublication(token1);

//            var response = await Client.PostAuthAsync($"{Route}/{id}/submit", token2);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        // ── PDF ────────────────────────────────────────────────────────────────

//        [Fact]
//        public async Task AddPdf_NonExistentPublication_Returns404()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_pdf404@test.com");
//            var response = await Client.PostAuthJsonAsync(
//                $"{Route}/{Guid.NewGuid()}/pdf",
//                new { pdfUrl = "https://example.com/phd.pdf" }, token);
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        [Fact]
//        public async Task AddPdf_OwnPublication_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_pdfok@test.com");
//            var id = await CreateSubmittedPublication(token);

//            var response = await Client.PostAuthJsonAsync(
//                $"{Route}/{id}/pdf",
//                new { pdfUrl = "https://example.com/phd.pdf" }, token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task AddPdf_OtherOwnersPublication_Returns403()
//        {
//            var token1 = await LoginAsNewUser(UserRole.PhDStudent, "phd_pdfow1@test.com");
//            var token2 = await LoginAsNewUser(UserRole.PhDStudent, "phd_pdfow2@test.com");
//            var id = await CreateSubmittedPublication(token1);

//            var response = await Client.PostAuthJsonAsync(
//                $"{Route}/{id}/pdf",
//                new { pdfUrl = "https://example.com/phd.pdf" }, token2);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        [Fact]
//        public async Task RemovePdf_NonExistentPublication_Returns404()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_rm404@test.com");
//            var response = await Client.DeleteAuthJsonAsync(
//                $"{Route}/{Guid.NewGuid()}/pdf",
//                new { pdfUrl = "https://example.com/phd.pdf" }, token);
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }

//        [Fact]
//        public async Task RemovePdf_OtherOwnersPublication_Returns403()
//        {
//            var token1 = await LoginAsNewUser(UserRole.PhDStudent, "phd_rmow1@test.com");
//            var token2 = await LoginAsNewUser(UserRole.PhDStudent, "phd_rmow2@test.com");
//            var id = await CreateSubmittedPublication(token1);

//            var response = await Client.DeleteAuthJsonAsync(
//                $"{Route}/{id}/pdf",
//                new { pdfUrl = "https://example.com/phd.pdf" }, token2);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        // ─── Helpers ──────────────────────────────────────────────────────────

//        private async Task<Guid> CreateSubmittedPublication(string? token)
//        {
//            var response = await Client.PostAuthJsonAsync(
//                Route, PublicationPayloads.ConferencePaper(), token);
//            response.EnsureSuccessStatusCode();
//            var body = await response.Content.ReadFromJsonAsync<CreatePublicationResponse>();
//            return body!.Publication.Id;
//        }

//        private async Task<string?> LoginAsNewUser(UserRole role, string email)
//        {
//            var adminToken = await LoginAsSuperAdmin();
//            var addResponse = await Client.AddUser(new CreateUserRequest
//            {
//                FirstName = role.ToString(),
//                LastName = "User",
//                Email = email,
//                Password = "TestPass1!",
//                Role = role,
//                IsActive = true
//            }, (string?)adminToken);
//            addResponse.EnsureSuccessStatusCode();
//            return (await Client.AuthenticateUser(new LoginRequest(email, "TestPass1!"), "v1"))?.AccessToken;
//        }
//    }

//    // ══════════════════════════════════════════════════════════════════════════
//    // 4. PhD Student — All Lab Publications (read-only)
//    //    Route: /api/v1/dashboard/phd-student/all-publications
//    // ══════════════════════════════════════════════════════════════════════════

//    [Collection("E2E collection")]
//    public class PhDStudentLabPublicationsE2ETests : BaseE2ETests
//    {
//        private const string Route = "/api/v1/dashboard/phd-student/all-publications";

//        public PhDStudentLabPublicationsE2ETests(PostgresFixture db, MailHogFixture mail)
//            : base(db, mail) { }

//        [Fact]
//        public async Task Get_Unauthenticated_Returns401()
//        {
//            var response = await Client.GetAsync(Route);
//            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_AsResearcher_Returns403()
//        {
//            var token = await LoginAsNewUser(UserRole.Researcher, "phd_lab_res@test.com");
//            var response = await Client.GetAuthAsync(Route, token);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_AsMasterian_Returns403()
//        {
//            var token = await LoginAsNewUser(UserRole.Masterian, "phd_lab_master@test.com");
//            var response = await Client.GetAuthAsync(Route, token);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_AsPhDStudent_Returns200WithShape()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_lab_get@test.com");
//            var body = await Client.GetAuthAsync(Route, token)
//                                    .ReadAs<PublicationsListResponse>();

//            Assert.NotNull(body?.Data);
//            Assert.NotNull(body?.Pagination);
//        }

//        [Fact]
//        public async Task Get_WithTypeFilter_ArticleJournal_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_lab_type@test.com");
//            var response = await Client.GetAuthAsync(
//                $"{Route}?type={nameof(PublicationType.ArticleJournal)}", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_WithTypeFilter_ChapterBook_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_lab_chap@test.com");
//            var response = await Client.GetAuthAsync(
//                $"{Route}?type={nameof(PublicationType.ChapterBook)}", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_WithTypeFilter_TechnicalReport_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_lab_rep@test.com");
//            var response = await Client.GetAuthAsync(
//                $"{Route}?type={nameof(PublicationType.TechnicalReport)}", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_WithVisibilityFilter_Private_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_lab_priv@test.com");
//            var response = await Client.GetAuthAsync(
//                $"{Route}?visibility={nameof(PublicationVisibility.Private)}", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_WithVisibilityFilter_Public_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_lab_pub@test.com");
//            var response = await Client.GetAuthAsync(
//                $"{Route}?visibility={nameof(PublicationVisibility.Public)}", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_WithYearAndSearchFilters_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_lab_year@test.com");
//            var response = await Client.GetAuthAsync(
//                $"{Route}?year=2023&search=neural", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_WithPagination_ReflectsRequestedPageAndLimit()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_lab_page@test.com");
//            var body = await Client.GetAuthAsync($"{Route}?page=1&limit=5", token)
//                                    .ReadAs<PublicationsListResponse>();

//            Assert.Equal(1, body!.Pagination.Page);
//            Assert.Equal(5, body.Pagination.Limit);
//        }

//        [Fact]
//        public async Task Get_WithAxeId_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "phd_lab_axe@test.com");
//            var response = await Client.GetAuthAsync(
//                $"{Route}?axeId={Guid.NewGuid()}", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        // ─── Helper ───────────────────────────────────────────────────────────

//        private async Task<string?> LoginAsNewUser(UserRole role, string email)
//        {
//            var adminToken = await LoginAsSuperAdmin();
//            var addResponse = await Client.AddUser(new CreateUserRequest
//            {
//                FirstName = role.ToString(),
//                LastName = "User",
//                Email = email,
//                Password = "TestPass1!",
//                Role = role,
//                IsActive = true
//            }, (string?)adminToken);
//            addResponse.EnsureSuccessStatusCode();
//            return (await Client.AuthenticateUser(new LoginRequest(email, "TestPass1!"), "v1"))?.AccessToken;
//        }
//    }

//    // ══════════════════════════════════════════════════════════════════════════
//    // 5. Masterian — All Lab Publications (read-only)
//    //    Route: /api/v1/dashboard/masterian/all-publications
//    // ══════════════════════════════════════════════════════════════════════════

//    [Collection("E2E collection")]
//    public class MasterianLabPublicationsE2ETests : BaseE2ETests
//    {
//        private const string Route = "/api/v1/dashboard/master-student/all-publications";

//        public MasterianLabPublicationsE2ETests(PostgresFixture db, MailHogFixture mail)
//            : base(db, mail) { }

//        [Fact]
//        public async Task Get_Unauthenticated_Returns401()
//        {
//            var response = await Client.GetAsync(Route);
//            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_AsPhDStudent_Returns403()
//        {
//            var token = await LoginAsNewUser(UserRole.PhDStudent, "master_lab_phd@test.com");
//            var response = await Client.GetAuthAsync(Route, token);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_AsResearcher_Returns403()
//        {
//            var token = await LoginAsNewUser(UserRole.Researcher, "master_lab_res@test.com");
//            var response = await Client.GetAuthAsync(Route, token);
//            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_AsMasterian_Returns200WithShape()
//        {
//            var token = await LoginAsNewUser(UserRole.Masterian, "master_lab_get@test.com");
//            var body = await Client.GetAuthAsync(Route, token)
//                                    .ReadAs<PublicationsListResponse>();

//            Assert.NotNull(body?.Data);
//            Assert.NotNull(body?.Pagination);
//        }

//        [Fact]
//        public async Task Get_WithTypeFilter_ConferenceNational_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.Masterian, "master_lab_cn@test.com");
//            var response = await Client.GetAuthAsync(
//                $"{Route}?type={nameof(PublicationType.ConferenceNational)}", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_WithTypeFilter_ChapterBook_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.Masterian, "master_lab_chap@test.com");
//            var response = await Client.GetAuthAsync(
//                $"{Route}?type={nameof(PublicationType.ChapterBook)}", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_WithTypeFilter_TechnicalReport_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.Masterian, "master_lab_rep@test.com");
//            var response = await Client.GetAuthAsync(
//                $"{Route}?type={nameof(PublicationType.TechnicalReport)}", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_WithVisibilityFilter_Private_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.Masterian, "master_lab_priv@test.com");
//            var response = await Client.GetAuthAsync(
//                $"{Route}?visibility={nameof(PublicationVisibility.Private)}", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_WithYearFilter_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.Masterian, "master_lab_yr@test.com");
//            var response = await Client.GetAuthAsync($"{Route}?year=2024", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_WithSearchFilter_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.Masterian, "master_lab_srch@test.com");
//            var response = await Client.GetAuthAsync($"{Route}?search=deep+learning", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async Task Get_WithPagination_ReflectsRequestedPageAndLimit()
//        {
//            var token = await LoginAsNewUser(UserRole.Masterian, "master_lab_pg@test.com");
//            var body = await Client.GetAuthAsync($"{Route}?page=2&limit=5", token)
//                                    .ReadAs<PublicationsListResponse>();

//            Assert.Equal(2, body!.Pagination.Page);
//            Assert.Equal(5, body.Pagination.Limit);
//        }

//        [Fact]
//        public async Task Get_WithAxeId_Returns200()
//        {
//            var token = await LoginAsNewUser(UserRole.Masterian, "master_lab_axe@test.com");
//            var response = await Client.GetAuthAsync($"{Route}?axeId={Guid.NewGuid()}", token);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        // ─── Helper ───────────────────────────────────────────────────────────

//        private async Task<string?> LoginAsNewUser(UserRole role, string email)
//        {
//            var adminToken = await LoginAsSuperAdmin();
//            var addResponse = await Client.AddUser(new CreateUserRequest
//            {
//                FirstName = role.ToString(),
//                LastName = "User",
//                Email = email,
//                Password = "TestPass1!",
//                Role = role,
//                IsActive = true
//            }, (string?)adminToken);
//            addResponse.EnsureSuccessStatusCode();
//            return (await Client.AuthenticateUser(new LoginRequest(email, "TestPass1!"), "v1"))?.AccessToken;
//        }
//    }

//    // ══════════════════════════════════════════════════════════════════════════
//    // Local response DTOs  (file-scoped → no clash with production models)
//    // ══════════════════════════════════════════════════════════════════════════

//    file record PublicationsListResponse(
//        List<PublicationSummary> Data,
//        PublicationStats Stats,
//        PaginationInfo Pagination);

//    file record PublicationSummary(
//        Guid Id,
//        string Type,
//        string Title,
//        int Year,
//        string? Status,
//        string? Venue,
//        string? Doi);

//    file record PublicationDetail(
//        Guid Id,
//        string Type,
//        string Status,
//        string Visibility,
//        string Title,
//        int Year);

//    file record PublicationStats(int Total, int Journals, int Conferences);

//    file record PaginationInfo(int Total, int Page, int Limit, int TotalPages);

//    file record CreatePublicationResponse(
//        string Message,
//        CreatedPublicationRef Publication);

//    file record CreatedPublicationRef(Guid Id, string Status);

//    // ══════════════════════════════════════════════════════════════════════════
//    // Task<HttpResponseMessage> → T  convenience
//    // ══════════════════════════════════════════════════════════════════════════

//    file static class HttpResponseMessageExtensions
//    {
//        public static async Task<T?> ReadAs<T>(this Task<HttpResponseMessage> responseTask)
//        {
//            var response = await responseTask;
//            response.EnsureSuccessStatusCode();
//            return await response.Content.ReadFromJsonAsync<T>();
//        }
//    }
//}