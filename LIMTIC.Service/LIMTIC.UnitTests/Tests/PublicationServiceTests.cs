using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.Services.Publication;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;
using Moq;

namespace LIMTIC.UnitTests.Tests
{
    // ══════════════════════════════════════════════════════════════════════════
    // HOW MOCKING WORKS HERE
    // ══════════════════════════════════════════════════════════════════════════
    //
    // Instead of a real database we create Mock<IPublicationRepository> objects.
    // A mock is a fake implementation of an interface that:
    //   • Returns whatever we tell it to (via .Setup(...).Returns(...))
    //   • Records every call so we can verify it happened (via .Verify(...))
    //
    // The service under test (PublicationService) receives these mocks through
    // its constructor — it has no idea they are fakes.
    //
    // Each test:
    //   1. Arrange — configure mock return values for the scenario
    //   2. Act     — call the real service method
    //   3. Assert  — check the result AND verify the right repo calls were made
    //
    // IMPORTANT — GetFullDtoOrThrowAsync
    // ──────────────────────────────────
    // CreateAsync, UpdateAsync, UpdateVisibilityAsync, SubmitAsync, ApproveAsync,
    // RejectAsync, AddPdfAsync, and RemovePdfAsync all call GetFullDtoOrThrowAsync
    // internally after the write, which calls GetByIdWithDetailsAsync.
    // Every test for these methods must therefore set up that second mock call,
    // otherwise the helper throws InvalidOperationException before the assertion.
    // ══════════════════════════════════════════════════════════════════════════

    public class PublicationServiceTests
    {
        // ─── Mocks — one per repository interface ────────────────────────────

        private readonly Mock<IPublicationRepository> _pubRepo = new();
        private readonly Mock<IJournalArticleRepository> _journalRepo = new();
        private readonly Mock<ITechnicalReportRepository> _reportRepo = new();
        private readonly Mock<IBookChapterRepository> _chapterRepo = new();
        private readonly Mock<INationalConferenceRepository> _nationalRepo = new();
        private readonly Mock<IInternationalConferenceRepository> _intlRepo = new();

        // ─── System Under Test ───────────────────────────────────────────────

        // Built once per test instance; each test gets fresh mocks because
        // xUnit creates a new class instance for every [Fact].
        private readonly PublicationService _sut;

        public PublicationServiceTests()
        {
            _sut = new PublicationService(
                _pubRepo.Object,
                _journalRepo.Object,
                _reportRepo.Object,
                _chapterRepo.Object,
                _nationalRepo.Object,
                _intlRepo.Object);
        }

        // ─── Builder helper ──────────────────────────────────────────────────

        private static PublicationEntity BuildPublication(
            PublicationType type = PublicationType.ArticleJournal,
            PublicationStatus status = PublicationStatus.Draft,
            PublicationVisibility visibility = PublicationVisibility.Public,
            int year = 2024,
            Guid? userId = null,
            Guid? researchAxisId = null,
            string title = "Test Publication",
            DateTime? createdAt = null) => new()
            {
                Id = Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                ResearchAxisId = researchAxisId ?? Guid.NewGuid(),
                Title = title,
                Abstract = "Test abstract.",
                Keywords = ["AI", "ML"],
                Authors = ["Author One", "Author Two"],
                AttachedPdfs = [],
                Type = type,
                Status = status,
                Visibility = visibility,
                Year = year,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = createdAt ?? DateTime.UtcNow,
            };

        /// <summary>
        /// Sets up GetByIdWithDetailsAsync so that GetFullDtoOrThrowAsync (called
        /// internally by every write method) succeeds and returns a mapped DTO.
        /// Call this in every test for CreateAsync, UpdateAsync, SubmitAsync,
        /// ApproveAsync, RejectAsync, UpdateVisibilityAsync, AddPdfAsync, and
        /// RemovePdfAsync.
        /// </summary>
        private void SetupGetByIdWithDetails(PublicationEntity publication) =>
            _pubRepo
                .Setup(r => r.GetByIdWithDetailsAsync(publication.Id, default))
                .ReturnsAsync(publication);

        // ══════════════════════════════════════════════════════════════════════
        // GET BY ID
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsPublication()
        {
            // Arrange
            var publication = BuildPublication(title: "My Publication");
            _pubRepo
                .Setup(r => r.GetByIdWithDetailsAsync(publication.Id, default))
                .ReturnsAsync(publication);

            // Act
            var result = await _sut.GetByIdAsync(publication.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(publication.Id, result.Id);
            Assert.Equal("My Publication", result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            // Arrange — mock returns null for any Guid (nothing found)
            _pubRepo
                .Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            // Act
            var result = await _sut.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        // ══════════════════════════════════════════════════════════════════════
        // GET BY USER ID
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task GetByUserIdAsync_ReturnsOnlyThatUsersPublications()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var list = new List<PublicationEntity>
            {
                BuildPublication(userId: userId, title: "Pub 1"),
                BuildPublication(userId: userId, title: "Pub 2"),
            };
            _pubRepo
                .Setup(r => r.GetByUserIdAsync(userId, default))
                .ReturnsAsync(list);

            // Act
            var result = await _sut.GetByUserIdAsync(userId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal(userId, p.UserId));
        }

        [Fact]
        public async Task GetByUserIdAsync_UserWithNoPublications_ReturnsEmpty()
        {
            // Arrange
            _pubRepo
                .Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(new List<PublicationEntity>());

            // Act
            var result = await _sut.GetByUserIdAsync(Guid.NewGuid());

            // Assert
            Assert.Empty(result);
        }

        // ══════════════════════════════════════════════════════════════════════
        // GET BY USER ID AND STATUS
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task GetByUserIdAndStatusAsync_ReturnsOnlyMatchingStatusPublications()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var drafts = new List<PublicationEntity>
            {
                BuildPublication(userId: userId, status: PublicationStatus.Draft, title: "Draft Pub")
            };
            _pubRepo
                .Setup(r => r.GetByUserIdAndStatusAsync(userId, PublicationStatus.Draft, default))
                .ReturnsAsync(drafts);

            // Act
            var result = await _sut.GetByUserIdAndStatusAsync(userId, PublicationStatus.Draft);

            // Assert
            Assert.Single(result);
            Assert.Equal(PublicationStatus.Draft, result.First().Status);
        }

        // ══════════════════════════════════════════════════════════════════════
        // GET PUBLIC PUBLICATIONS
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task GetPublicPublicationsAsync_CallsRepoWithPublishedAndPublic()
        {
            // Arrange
            var expected = new List<PublicationEntity>
            {
                BuildPublication(status: PublicationStatus.Published, visibility: PublicationVisibility.Public)
            };
            _pubRepo
                .Setup(r => r.GetByStatusAndVisibilityAsync(
                    PublicationStatus.Published, PublicationVisibility.Public, default))
                .ReturnsAsync(expected);

            // Act
            var result = await _sut.GetPublicPublicationsAsync();

            // Assert — result is correct AND repo was called with the right arguments
            Assert.Single(result);
            _pubRepo.Verify(r => r.GetByStatusAndVisibilityAsync(
                PublicationStatus.Published, PublicationVisibility.Public, default), Times.Once);
        }

        // ══════════════════════════════════════════════════════════════════════
        // GET RECENT PUBLIC PUBLICATIONS
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task GetRecentPublicPublicationsAsync_RespectsLimit()
        {
            // Arrange — repo returns 5, service must trim to 3
            var allPublic = Enumerable.Range(0, 5)
                .Select(i => BuildPublication(
                    status: PublicationStatus.Published,
                    visibility: PublicationVisibility.Public,
                    year: 2020 + i,
                    title: $"Pub {i}"))
                .ToList();

            _pubRepo
                .Setup(r => r.GetByStatusAndVisibilityAsync(
                    PublicationStatus.Published, PublicationVisibility.Public, default))
                .ReturnsAsync(allPublic);

            // Act
            var result = await _sut.GetRecentPublicPublicationsAsync(limit: 3);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetRecentPublicPublicationsAsync_ReturnsMostRecentFirst()
        {
            // Arrange — deliberately give them out-of-order to confirm service sorts them
            var publications = new List<PublicationEntity>
            {
                BuildPublication(year: 2021, title: "Oldest",  createdAt: new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                BuildPublication(year: 2023, title: "Newest",  createdAt: new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                BuildPublication(year: 2022, title: "Middle",  createdAt: new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            };

            _pubRepo
                .Setup(r => r.GetByStatusAndVisibilityAsync(
                    PublicationStatus.Published, PublicationVisibility.Public, default))
                .ReturnsAsync(publications);

            // Act
            var result = (await _sut.GetRecentPublicPublicationsAsync(limit: 2)).ToList();

            // Assert
            Assert.Equal(2023, result[0].Year);
            Assert.Equal(2022, result[1].Year);
        }

        [Fact]
        public async Task GetRecentPublicPublicationsAsync_SameYear_OrdersByCreatedAtDescending()
        {
            // Arrange — same Year, different CreatedAtUtc; confirms ThenByDescending branch
            var earlier = BuildPublication(year: 2024, title: "Earlier",
                createdAt: new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc));
            var later = BuildPublication(year: 2024, title: "Later",
                createdAt: new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Utc));

            _pubRepo
                .Setup(r => r.GetByStatusAndVisibilityAsync(
                    PublicationStatus.Published, PublicationVisibility.Public, default))
                .ReturnsAsync([earlier, later]);

            // Act
            var result = (await _sut.GetRecentPublicPublicationsAsync(limit: 2)).ToList();

            // Assert — "Later" (Sept) should come before "Earlier" (Mar)
            Assert.Equal("Later", result[0].Title);
            Assert.Equal("Earlier", result[1].Title);
        }

        // ══════════════════════════════════════════════════════════════════════
        // GET BY RESEARCH AXIS
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task GetByResearchAxisIdAsync_ReturnsMockedList()
        {
            // Arrange
            var axisId = Guid.NewGuid();
            var list = new List<PublicationEntity>
            {
                BuildPublication(researchAxisId: axisId, title: "Axis Pub 1"),
                BuildPublication(researchAxisId: axisId, title: "Axis Pub 2"),
            };
            _pubRepo
                .Setup(r => r.GetByResearchAxisIdAsync(axisId, default))
                .ReturnsAsync(list);

            // Act
            var result = await _sut.GetByResearchAxisIdAsync(axisId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal(axisId, p.ResearchAxisId));
        }

        // ══════════════════════════════════════════════════════════════════════
        // GET BY TYPE
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task GetByTypeAsync_ReturnsOnlyPublicationsOfThatType()
        {
            // Arrange
            var list = new List<PublicationEntity>
            {
                BuildPublication(type: PublicationType.ArticleJournal, title: "Journal 1"),
                BuildPublication(type: PublicationType.ArticleJournal, title: "Journal 2"),
            };
            _pubRepo
                .Setup(r => r.GetByTypeAsync(PublicationType.ArticleJournal, default))
                .ReturnsAsync(list);

            // Act
            var result = await _sut.GetByTypeAsync(PublicationType.ArticleJournal);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal(PublicationType.ArticleJournal, p.Type));
        }

        // ══════════════════════════════════════════════════════════════════════
        // GET FILTERED
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task GetFilteredAsync_ForwardsAllParametersToRepository()
        {
            // Arrange — verify the service passes every parameter straight through
            var expected = (Items: (IEnumerable<PublicationEntity>)new List<PublicationEntity>(), TotalCount: 0);
            _pubRepo
                .Setup(r => r.GetFilteredAsync(
                    PublicationType.ArticleJournal,
                    PublicationStatus.Published,
                    PublicationVisibility.Public,
                    It.IsAny<Guid?>(),
                    It.IsAny<Guid?>(),
                    2023,
                    "quantum",
                    2,
                    10,
                    default))
                .ReturnsAsync(expected);

            // Act
            await _sut.GetFilteredAsync(
                type: PublicationType.ArticleJournal,
                status: PublicationStatus.Published,
                visibility: PublicationVisibility.Public,
                year: 2023,
                search: "quantum",
                page: 2,
                pageSize: 10);

            // Assert — the repo was called with exactly those values
            _pubRepo.Verify(r => r.GetFilteredAsync(
                PublicationType.ArticleJournal,
                PublicationStatus.Published,
                PublicationVisibility.Public,
                It.IsAny<Guid?>(),
                It.IsAny<Guid?>(),
                2023,
                "quantum",
                2,
                10,
                default), Times.Once);
        }

        [Fact]
        public async Task GetFilteredAsync_ReturnsTotalCountFromRepository()
        {
            // Arrange
            var items = new List<PublicationEntity> { BuildPublication(), BuildPublication() };
            var expected = (Items: (IEnumerable<PublicationEntity>)items, TotalCount: 42);
            _pubRepo
                .Setup(r => r.GetFilteredAsync(
                    It.IsAny<PublicationType?>(), It.IsAny<PublicationStatus?>(),
                    It.IsAny<PublicationVisibility?>(), It.IsAny<Guid?>(),
                    It.IsAny<Guid?>(), It.IsAny<int?>(), It.IsAny<string?>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            // Act
            var (resultItems, totalCount) = await _sut.GetFilteredAsync();

            // Assert
            Assert.Equal(42, totalCount);
            Assert.Equal(2, resultItems.Count());
        }

        // ══════════════════════════════════════════════════════════════════════
        // TYPE-SPECIFIC DETAIL ACCESSORS
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task GetJournalArticleByPublicationIdAsync_ExistingId_ReturnsMappedDto()
        {
            var pubId = Guid.NewGuid();
            var article = new JournalArticleEntity
            {
                Id = Guid.NewGuid(),
                JournalName = "Nature",
                Volume = "12",
                Number = "3",
                Pages = "100-110",
                Ranking = JournalRanking.Q1,
            };
            _journalRepo
                .Setup(r => r.GetByPublicationIdAsync(pubId, default))
                .ReturnsAsync(article);

            var result = await _sut.GetJournalArticleByPublicationIdAsync(pubId);

            Assert.NotNull(result);
            Assert.Equal(article.Id, result.Id);
            Assert.Equal("Nature", result.JournalName);
        }

        [Fact]
        public async Task GetJournalArticleByPublicationIdAsync_NonExistentId_ReturnsNull()
        {
            _journalRepo
                .Setup(r => r.GetByPublicationIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((JournalArticleEntity?)null);

            var result = await _sut.GetJournalArticleByPublicationIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetTechnicalReportByPublicationIdAsync_ExistingId_ReturnsMappedDto()
        {
            var pubId = Guid.NewGuid();
            var report = new TechnicalReportEntity
            {
                Id = Guid.NewGuid(),
                ReportNumber = 42,
                Institution = "MIT",
            };
            _reportRepo
                .Setup(r => r.GetByPublicationIdAsync(pubId, default))
                .ReturnsAsync(report);

            var result = await _sut.GetTechnicalReportByPublicationIdAsync(pubId);

            Assert.NotNull(result);
            Assert.Equal(42, result.ReportNumber);
        }

        [Fact]
        public async Task GetTechnicalReportByPublicationIdAsync_NonExistentId_ReturnsNull()
        {
            _reportRepo
                .Setup(r => r.GetByPublicationIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((TechnicalReportEntity?)null);

            var result = await _sut.GetTechnicalReportByPublicationIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetBookChapterByPublicationIdAsync_ExistingId_ReturnsMappedDto()
        {
            var pubId = Guid.NewGuid();
            var chapter = new BookChapterEntity
            {
                Id = Guid.NewGuid(),
                BookTitle = "Deep Learning",
                Publisher = "O'Reilly",
                Isbn = "978-1-491-91490-2",
                Pages = "100-130",
            };
            _chapterRepo
                .Setup(r => r.GetByPublicationIdAsync(pubId, default))
                .ReturnsAsync(chapter);

            var result = await _sut.GetBookChapterByPublicationIdAsync(pubId);

            Assert.NotNull(result);
            Assert.Equal("Deep Learning", result.BookTitle);
        }

        [Fact]
        public async Task GetBookChapterByPublicationIdAsync_NonExistentId_ReturnsNull()
        {
            _chapterRepo
                .Setup(r => r.GetByPublicationIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((BookChapterEntity?)null);

            var result = await _sut.GetBookChapterByPublicationIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetNationalConferenceByPublicationIdAsync_ExistingId_ReturnsMappedDto()
        {
            var pubId = Guid.NewGuid();
            var conf = new NationalConferenceEntity
            {
                Id = Guid.NewGuid(),
                ConferenceName = "TunisConf",
                Location = "Tunis",
                Pages = "5-15",
            };
            _nationalRepo
                .Setup(r => r.GetByPublicationIdAsync(pubId, default))
                .ReturnsAsync(conf);

            var result = await _sut.GetNationalConferenceByPublicationIdAsync(pubId);

            Assert.NotNull(result);
            Assert.Equal("TunisConf", result.ConferenceName);
        }

        [Fact]
        public async Task GetNationalConferenceByPublicationIdAsync_NonExistentId_ReturnsNull()
        {
            _nationalRepo
                .Setup(r => r.GetByPublicationIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((NationalConferenceEntity?)null);

            var result = await _sut.GetNationalConferenceByPublicationIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetInternationalConferenceByPublicationIdAsync_ExistingId_ReturnsMappedDto()
        {
            var pubId = Guid.NewGuid();
            var conf = new InternationalConferenceEntity
            {
                Id = Guid.NewGuid(),
                ConferenceName = "NeurIPS",
                Location = "Vancouver",
                Pages = "1-10",
                Ranking = CoreRanking.A,
            };
            _intlRepo
                .Setup(r => r.GetByPublicationIdAsync(pubId, default))
                .ReturnsAsync(conf);

            var result = await _sut.GetInternationalConferenceByPublicationIdAsync(pubId);

            Assert.NotNull(result);
            Assert.Equal("NeurIPS", result.ConferenceName);
            Assert.Equal(CoreRanking.A, result.Ranking);
        }

        [Fact]
        public async Task GetInternationalConferenceByPublicationIdAsync_NonExistentId_ReturnsNull()
        {
            _intlRepo
                .Setup(r => r.GetByPublicationIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((InternationalConferenceEntity?)null);

            var result = await _sut.GetInternationalConferenceByPublicationIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        // ══════════════════════════════════════════════════════════════════════
        // CREATE
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task CreateAsync_AlwaysSetsDraftStatus_RegardlessOfInput()
        {
            // Arrange — caller passes Published trying to bypass the workflow
            var publication = BuildPublication(status: PublicationStatus.Published);
            _pubRepo.Setup(r => r.AddAsync(It.IsAny<PublicationEntity>(), default)).Returns(Task.CompletedTask);
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            // Act
            var result = await _sut.CreateAsync(publication);

            // Assert — service must override whatever status was passed in
            Assert.Equal(PublicationStatus.Draft, result.Status);
        }

        [Fact]
        public async Task CreateAsync_CallsAddAndSaveChanges()
        {
            // Arrange
            var publication = BuildPublication();
            _pubRepo.Setup(r => r.AddAsync(publication, default)).Returns(Task.CompletedTask);
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            // Act
            await _sut.CreateAsync(publication);

            // Assert — both repo methods must have been called exactly once
            _pubRepo.Verify(r => r.AddAsync(publication, default), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        // ══════════════════════════════════════════════════════════════════════
        // UPDATE
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task UpdateAsync_DraftPublication_CallsUpdateAndSave()
        {
            // Arrange
            var publication = BuildPublication(status: PublicationStatus.Draft, title: "Original");
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(publication));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            publication.Title = "Updated";

            // Act
            var result = await _sut.UpdateAsync(publication);

            // Assert
            Assert.Equal("Updated", result.Title);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_RejectedPublication_IsAllowedToBeEdited()
        {
            // Arrange
            var publication = BuildPublication(status: PublicationStatus.Rejected);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            // Act & Assert — must not throw
            await _sut.UpdateAsync(publication);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_SubmittedPublication_ThrowsInvalidOperationException()
        {
            // Arrange
            var publication = BuildPublication(status: PublicationStatus.Submitted);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.UpdateAsync(publication));

            // Verify Update and SaveChanges were NEVER called
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_PublishedPublication_ThrowsInvalidOperationException()
        {
            // Arrange
            var publication = BuildPublication(status: PublicationStatus.Published);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.UpdateAsync(publication));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_NonExistentPublication_ThrowsKeyNotFoundException()
        {
            // Arrange
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.UpdateAsync(BuildPublication()));
        }

        // ══════════════════════════════════════════════════════════════════════
        // DELETE
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task DeleteAsync_ExistingPublication_CallsRemoveAndSave()
        {
            // Arrange
            var publication = BuildPublication(type: PublicationType.ArticleJournal);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Remove(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            // Act
            await _sut.DeleteAsync(publication.Id);

            // Assert
            _pubRepo.Verify(r => r.Remove(publication), Times.Once());
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_NonExistentId_ThrowsKeyNotFoundException()
        {
            // Arrange
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.DeleteAsync(Guid.NewGuid()));

            _pubRepo.Verify(r => r.Remove(It.IsAny<PublicationEntity>()), Times.Never());
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never());
        }

        // ══════════════════════════════════════════════════════════════════════
        // UPDATE VISIBILITY
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task UpdateVisibilityAsync_ExistingPublication_ChangesVisibilityAndSaves()
        {
            // Arrange
            var publication = BuildPublication(visibility: PublicationVisibility.Public);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            // Act
            var result = await _sut.UpdateVisibilityAsync(publication.Id, PublicationVisibility.Private);

            // Assert — the entity's visibility was mutated before Update was called
            Assert.Equal(PublicationVisibility.Private, result.Visibility);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateVisibilityAsync_NonExistentPublication_ThrowsKeyNotFoundException()
        {
            // Arrange
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.UpdateVisibilityAsync(Guid.NewGuid(), PublicationVisibility.Private));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        // ══════════════════════════════════════════════════════════════════════
        // SUBMIT
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task SubmitAsync_DraftPublication_TransitionsToSubmitted()
        {
            // Arrange
            var publication = BuildPublication(status: PublicationStatus.Draft);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            // Act
            var result = await _sut.SubmitAsync(publication.Id);

            // Assert
            Assert.Equal(PublicationStatus.Submitted, result.Status);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task SubmitAsync_NonDraftPublication_ThrowsInvalidOperationException()
        {
            // Arrange — try to submit something already Submitted
            var publication = BuildPublication(status: PublicationStatus.Submitted);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.SubmitAsync(publication.Id));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task SubmitAsync_PublishedPublication_ThrowsInvalidOperationException()
        {
            // Arrange
            var publication = BuildPublication(status: PublicationStatus.Published);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.SubmitAsync(publication.Id));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task SubmitAsync_NonExistentPublication_ThrowsKeyNotFoundException()
        {
            // Arrange
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.SubmitAsync(Guid.NewGuid()));
        }

        // ══════════════════════════════════════════════════════════════════════
        // APPROVE
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task ApproveAsync_SubmittedPublication_TransitionsToPublished()
        {
            // Arrange
            var publication = BuildPublication(status: PublicationStatus.Submitted);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            // Act
            var result = await _sut.ApproveAsync(publication.Id);

            // Assert
            Assert.Equal(PublicationStatus.Published, result.Status);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task ApproveAsync_DraftPublication_ThrowsInvalidOperationException()
        {
            // Arrange — only Submitted publications may be approved
            var publication = BuildPublication(status: PublicationStatus.Draft);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.ApproveAsync(publication.Id));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task ApproveAsync_AlreadyPublishedPublication_ThrowsInvalidOperationException()
        {
            // Arrange
            var publication = BuildPublication(status: PublicationStatus.Published);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.ApproveAsync(publication.Id));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task ApproveAsync_NonExistentPublication_ThrowsKeyNotFoundException()
        {
            // Arrange
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.ApproveAsync(Guid.NewGuid()));
        }

        // ══════════════════════════════════════════════════════════════════════
        // REJECT
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task RejectAsync_SubmittedPublication_TransitionsToRejected()
        {
            // Arrange
            var publication = BuildPublication(status: PublicationStatus.Submitted);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            // Act
            var result = await _sut.RejectAsync(publication.Id, reason: "Insufficient references.");

            // Assert
            Assert.Equal(PublicationStatus.Rejected, result.Status);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task RejectAsync_DraftPublication_ThrowsInvalidOperationException()
        {
            // Arrange — only Submitted publications may be rejected
            var publication = BuildPublication(status: PublicationStatus.Draft);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.RejectAsync(publication.Id));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task RejectAsync_AlreadyRejectedPublication_ThrowsInvalidOperationException()
        {
            // Arrange
            var publication = BuildPublication(status: PublicationStatus.Rejected);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.RejectAsync(publication.Id));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task RejectAsync_NonExistentPublication_ThrowsKeyNotFoundException()
        {
            // Arrange
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.RejectAsync(Guid.NewGuid()));
        }

        // ══════════════════════════════════════════════════════════════════════
        // PDF MANAGEMENT
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task AddPdfAsync_ExistingPublication_AppendsPdfUrlAndSaves()
        {
            // Arrange
            const string newPdf = "https://storage/paper.pdf";
            var publication = BuildPublication();
            publication.AttachedPdfs = ["https://storage/existing.pdf"];

            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            // Act
            var result = await _sut.AddPdfAsync(publication.Id, newPdf);

            // Assert — both the old and new URL should be present
            Assert.Contains(newPdf, result.AttachedPdfs);
            Assert.Contains("https://storage/existing.pdf", result.AttachedPdfs);
            Assert.Equal(2, result.AttachedPdfs.Length);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task AddPdfAsync_NonExistentPublication_ThrowsKeyNotFoundException()
        {
            // Arrange
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.AddPdfAsync(Guid.NewGuid(), "https://storage/paper.pdf"));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task RemovePdfAsync_ExistingPublication_RemovesMatchingUrlAndSaves()
        {
            // Arrange
            const string target = "https://storage/remove-me.pdf";
            const string keep = "https://storage/keep-me.pdf";
            var publication = BuildPublication();
            publication.AttachedPdfs = [target, keep];

            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            // Act
            var result = await _sut.RemovePdfAsync(publication.Id, target);

            // Assert
            Assert.DoesNotContain(target, result.AttachedPdfs);
            Assert.Contains(keep, result.AttachedPdfs);
            Assert.Single(result.AttachedPdfs);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task RemovePdfAsync_NonExistentPublication_ThrowsKeyNotFoundException()
        {
            // Arrange
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.RemovePdfAsync(Guid.NewGuid(), "https://storage/paper.pdf"));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task RemovePdfAsync_UrlNotInList_LeavesArrayUnchangedAndStillSaves()
        {
            // Arrange — removing a URL that was never there should not throw
            const string keep = "https://storage/keep-me.pdf";
            var publication = BuildPublication();
            publication.AttachedPdfs = [keep];

            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            // Act
            var result = await _sut.RemovePdfAsync(publication.Id, "https://storage/ghost.pdf");

            // Assert — list is unchanged, no exception
            Assert.Single(result.AttachedPdfs);
            Assert.Contains(keep, result.AttachedPdfs);
        }

        // ══════════════════════════════════════════════════════════════════════
        // STATISTICS
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task CountPublicPublicationsAsync_DelegatesToRepoWithCorrectArguments()
        {
            // Arrange
            _pubRepo
                .Setup(r => r.CountByStatusAndVisibilityAsync(
                    PublicationStatus.Published, PublicationVisibility.Public, default))
                .ReturnsAsync(17);

            // Act
            var count = await _sut.CountPublicPublicationsAsync();

            // Assert
            Assert.Equal(17, count);
            _pubRepo.Verify(r => r.CountByStatusAndVisibilityAsync(
                PublicationStatus.Published, PublicationVisibility.Public, default), Times.Once);
        }

        [Fact]
        public async Task CountPublicPublicationsAsync_WhenNoneExist_ReturnsZero()
        {
            // Arrange
            _pubRepo
                .Setup(r => r.CountByStatusAndVisibilityAsync(
                    PublicationStatus.Published, PublicationVisibility.Public, default))
                .ReturnsAsync(0);

            // Act
            var count = await _sut.CountPublicPublicationsAsync();

            // Assert
            Assert.Equal(0, count);
        }
    }

    // ── Private helper so BuildPublication can accept createdAt without
    //    polluting the main class signature ──────────────────────────────────
    file static class PublicationEntityExtensions
    {
        public static PublicationEntity BuildPublication(
            PublicationType type = PublicationType.ArticleJournal,
            PublicationStatus status = PublicationStatus.Draft,
            PublicationVisibility visibility = PublicationVisibility.Public,
            int year = 2024,
            Guid? userId = null,
            Guid? researchAxisId = null,
            string title = "Test Publication",
            DateTime? createdAt = null) => new()
            {
                Id = Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                ResearchAxisId = researchAxisId ?? Guid.NewGuid(),
                Title = title,
                Abstract = "Test abstract.",
                Keywords = ["AI", "ML"],
                Authors = ["Author One", "Author Two"],
                AttachedPdfs = [],
                Type = type,
                Status = status,
                Visibility = visibility,
                Year = year,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = createdAt ?? DateTime.UtcNow,
            };
    }
}