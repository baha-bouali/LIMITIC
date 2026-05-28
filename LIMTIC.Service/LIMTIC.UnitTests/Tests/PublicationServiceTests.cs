using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.Services.Publication;
using LIMTIC.Application.Abstractions;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;
using Moq;
using LIMTIC.Domain.Abstractions.Publications;

namespace LIMTIC.UnitTests.Tests
{
    #region Mocking Notes

    public class PublicationServiceTests
    {
        #region Mocks

        private readonly Mock<IPublicationRepository> _pubRepo = new();
        private readonly Mock<IJournalArticleRepository> _journalRepo = new();
        private readonly Mock<ITechnicalReportRepository> _reportRepo = new();
        private readonly Mock<IBookChapterRepository> _chapterRepo = new();
        private readonly Mock<INationalConferenceRepository> _nationalRepo = new();
        private readonly Mock<IInternationalConferenceRepository> _intlRepo = new();
        private readonly Mock<ICurrentUserService> _currentUserService = new();

        #endregion

        #region System Under Test

        private readonly PublicationService _sut;

        public PublicationServiceTests()
        {
            _currentUserService.SetupGet(x => x.UserId).Returns(Guid.Empty);
            _currentUserService.SetupGet(x => x.Role).Returns((string?)null);

            _sut = new PublicationService(
                _pubRepo.Object,
                _journalRepo.Object,
                _reportRepo.Object,
                _chapterRepo.Object,
                _nationalRepo.Object,
                _intlRepo.Object,
                _currentUserService.Object);
        }

        #endregion

        #region Builder Helper

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

        private void SetupGetByIdWithDetails(PublicationEntity publication) =>
            _pubRepo
                .Setup(r => r.GetByIdWithDetailsAsync(publication.Id, default))
                .ReturnsAsync(publication);

        #endregion

        #region Get By Id

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsPublication()
        {
            var publication = BuildPublication(title: "My Publication");
            _pubRepo
                .Setup(r => r.GetByIdWithDetailsAsync(publication.Id, default))
                .ReturnsAsync(publication);

            var result = await _sut.GetByIdAsync(publication.Id);

            Assert.NotNull(result);
            Assert.Equal(publication.Id, result.Id);
            Assert.Equal("My Publication", result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            _pubRepo
                .Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            var result = await _sut.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        #endregion

        #region Get By User Id

        [Fact]
        public async Task GetByUserIdAsync_ReturnsOnlyThatUsersPublications()
        {
            var userId = Guid.NewGuid();
            var list = new List<PublicationEntity>
            {
                BuildPublication(userId: userId, title: "Pub 1"),
                BuildPublication(userId: userId, title: "Pub 2"),
            };
            _pubRepo
                .Setup(r => r.GetByUserIdAsync(userId, default))
                .ReturnsAsync(list);

            var result = await _sut.GetByUserIdAsync(userId);

            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal(userId, p.UserId));
        }

        [Fact]
        public async Task GetByUserIdAsync_UserWithNoPublications_ReturnsEmpty()
        {
            _pubRepo
                .Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(new List<PublicationEntity>());

            var result = await _sut.GetByUserIdAsync(Guid.NewGuid());

            Assert.Empty(result);
        }

        #endregion

        #region Get By User Id And Status

        [Fact]
        public async Task GetByUserIdAndStatusAsync_ReturnsOnlyMatchingStatusPublications()
        {
            var userId = Guid.NewGuid();
            var drafts = new List<PublicationEntity>
            {
                BuildPublication(userId: userId, status: PublicationStatus.Draft, title: "Draft Pub")
            };
            _pubRepo
                .Setup(r => r.GetByUserIdAndStatusAsync(userId, PublicationStatus.Draft, default))
                .ReturnsAsync(drafts);

            var result = await _sut.GetByUserIdAndStatusAsync(userId, PublicationStatus.Draft);

            Assert.Single(result);
            Assert.Equal(PublicationStatus.Draft, result.First().Status);
        }

        #endregion

        #region Get Public Publications

        [Fact]
        public async Task GetPublicPublicationsAsync_CallsRepoWithPublishedAndPublic()
        {
            var expected = new List<PublicationEntity>
            {
                BuildPublication(status: PublicationStatus.Published, visibility: PublicationVisibility.Public)
            };
            _pubRepo
                .Setup(r => r.GetByStatusAndVisibilityAsync(
                    PublicationStatus.Published, PublicationVisibility.Public, default))
                .ReturnsAsync(expected);

            var result = await _sut.GetPublicPublicationsAsync();

            Assert.Single(result);
            _pubRepo.Verify(r => r.GetByStatusAndVisibilityAsync(
                PublicationStatus.Published, PublicationVisibility.Public, default), Times.Once);
        }

        #endregion

        #region Get Recent Public Publications

        [Fact]
        public async Task GetRecentPublicPublicationsAsync_RespectsLimit()
        {
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

            var result = await _sut.GetRecentPublicPublicationsAsync(limit: 3);

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetRecentPublicPublicationsAsync_ReturnsMostRecentFirst()
        {
            var publications = new List<PublicationEntity>
            {
                BuildPublication(year: 2021, title: "Oldest", createdAt: new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                BuildPublication(year: 2023, title: "Newest", createdAt: new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                BuildPublication(year: 2022, title: "Middle", createdAt: new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            };

            _pubRepo
                .Setup(r => r.GetByStatusAndVisibilityAsync(
                    PublicationStatus.Published, PublicationVisibility.Public, default))
                .ReturnsAsync(publications);

            var result = (await _sut.GetRecentPublicPublicationsAsync(limit: 2)).ToList();

            Assert.Equal(2023, result[0].Year);
            Assert.Equal(2022, result[1].Year);
        }

        [Fact]
        public async Task GetRecentPublicPublicationsAsync_SameYear_OrdersByCreatedAtDescending()
        {
            var earlier = BuildPublication(year: 2024, title: "Earlier",
                createdAt: new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc));
            var later = BuildPublication(year: 2024, title: "Later",
                createdAt: new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Utc));

            _pubRepo
                .Setup(r => r.GetByStatusAndVisibilityAsync(
                    PublicationStatus.Published, PublicationVisibility.Public, default))
                .ReturnsAsync([earlier, later]);

            var result = (await _sut.GetRecentPublicPublicationsAsync(limit: 2)).ToList();

            Assert.Equal("Later", result[0].Title);
            Assert.Equal("Earlier", result[1].Title);
        }

        #endregion

        #region Get By Research Axis Id

        [Fact]
        public async Task GetByResearchAxisIdAsync_ReturnsMockedList()
        {
            var axisId = Guid.NewGuid();
            var list = new List<PublicationEntity>
            {
                BuildPublication(researchAxisId: axisId, title: "Axis Pub 1"),
                BuildPublication(researchAxisId: axisId, title: "Axis Pub 2"),
            };
            _pubRepo
                .Setup(r => r.GetByResearchAxisIdAsync(axisId, default))
                .ReturnsAsync(list);

            var result = await _sut.GetByResearchAxisIdAsync(axisId);

            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal(axisId, p.ResearchAxisId));
        }

        #endregion

        #region Get By Type

        [Fact]
        public async Task GetByTypeAsync_ReturnsOnlyPublicationsOfThatType()
        {
            var list = new List<PublicationEntity>
            {
                BuildPublication(type: PublicationType.ArticleJournal, title: "Journal 1"),
                BuildPublication(type: PublicationType.ArticleJournal, title: "Journal 2"),
            };
            _pubRepo
                .Setup(r => r.GetByTypeAsync(PublicationType.ArticleJournal, default))
                .ReturnsAsync(list);

            var result = await _sut.GetByTypeAsync(PublicationType.ArticleJournal);

            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal(PublicationType.ArticleJournal, p.Type));
        }

        #endregion

        #region GetFilteredAsync

        [Fact]
        public async Task GetFilteredAsync_ForwardsAllParametersToRepository()
        {
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

            await _sut.GetFilteredAsync(
                type: PublicationType.ArticleJournal,
                status: PublicationStatus.Published,
                visibility: PublicationVisibility.Public,
                year: 2023,
                search: "quantum",
                page: 2,
                pageSize: 10);

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
            var items = new List<PublicationEntity> { BuildPublication(), BuildPublication() };
            var expected = (Items: (IEnumerable<PublicationEntity>)items, TotalCount: 42);
            _pubRepo
                .Setup(r => r.GetFilteredAsync(
                    It.IsAny<PublicationType?>(), It.IsAny<PublicationStatus?>(),
                    It.IsAny<PublicationVisibility?>(), It.IsAny<Guid?>(),
                    It.IsAny<Guid?>(), It.IsAny<int?>(), It.IsAny<string?>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var (resultItems, totalCount) = await _sut.GetFilteredAsync();

            Assert.Equal(42, totalCount);
            Assert.Equal(2, resultItems.Count());
        }

        [Fact]
        public async Task GetFilteredAsync_WithNullVisibility_ReturnsAllPublications()
        {
            // When visibility is null, it means show all publications (both public and private)
            var items = new List<PublicationEntity>
            {
                BuildPublication(visibility: PublicationVisibility.Public),
                BuildPublication(visibility: PublicationVisibility.Private)
            };
            var expected = (Items: (IEnumerable<PublicationEntity>)items, TotalCount: 2);
            _pubRepo
                .Setup(r => r.GetFilteredAsync(
                    null, PublicationStatus.Published, null,
                    null, null, null, null, 1, 10, default))
                .ReturnsAsync(expected);

            var (resultItems, totalCount) = await _sut.GetFilteredAsync(
                type: null,
                status: PublicationStatus.Published,
                visibility: null,
                userId: null,
                researchAxisId: null,
                year: null,
                search: null,
                page: 1,
                pageSize: 10);

            Assert.Equal(2, totalCount);
            Assert.Equal(2, resultItems.Count());
            _pubRepo.Verify(r => r.GetFilteredAsync(
                null, PublicationStatus.Published, null,
                null, null, null, null, 1, 10, default), Times.Once);
        }

        [Fact]
        public async Task GetFilteredAsync_WithPublicVisibilityOnly_ReturnsPublicPublicationsOnly()
        {
            // When visibility is explicitly set to Public, show only public publications
            var items = new List<PublicationEntity>
            {
                BuildPublication(visibility: PublicationVisibility.Public),
                BuildPublication(visibility: PublicationVisibility.Public)
            };
            var expected = (Items: (IEnumerable<PublicationEntity>)items, TotalCount: 2);
            _pubRepo
                .Setup(r => r.GetFilteredAsync(
                    null, PublicationStatus.Published, PublicationVisibility.Public,
                    null, null, null, null, 1, 10, default))
                .ReturnsAsync(expected);

            var (resultItems, totalCount) = await _sut.GetFilteredAsync(
                type: null,
                status: PublicationStatus.Published,
                visibility: PublicationVisibility.Public,
                userId: null,
                researchAxisId: null,
                year: null,
                search: null,
                page: 1,
                pageSize: 10);

            Assert.Equal(2, totalCount);
            Assert.Equal(2, resultItems.Count());
            _pubRepo.Verify(r => r.GetFilteredAsync(
                null, PublicationStatus.Published, PublicationVisibility.Public,
                null, null, null, null, 1, 10, default), Times.Once);
        }

        [Fact]
        public async Task GetFilteredAsync_WithPrivateVisibility_ReturnsPrivatePublicationsOnly()
        {
            // When visibility is explicitly set to Private, show only private publications
            var items = new List<PublicationEntity>
            {
                BuildPublication(visibility: PublicationVisibility.Private)
            };
            var expected = (Items: (IEnumerable<PublicationEntity>)items, TotalCount: 1);
            _pubRepo
                .Setup(r => r.GetFilteredAsync(
                    null, PublicationStatus.Published, PublicationVisibility.Private,
                    null, null, null, null, 1, 10, default))
                .ReturnsAsync(expected);

            var (resultItems, totalCount) = await _sut.GetFilteredAsync(
                type: null,
                status: PublicationStatus.Published,
                visibility: PublicationVisibility.Private,
                userId: null,
                researchAxisId: null,
                year: null,
                search: null,
                page: 1,
                pageSize: 10);

            Assert.Equal(1, totalCount);
            Assert.Single(resultItems);
            _pubRepo.Verify(r => r.GetFilteredAsync(
                null, PublicationStatus.Published, PublicationVisibility.Private,
                null, null, null, null, 1, 10, default), Times.Once);
        }

        #endregion

        #region Type Specific

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

        #endregion

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_AlwaysSetsDraftStatus_RegardlessOfInput()
        {
            var publication = BuildPublication(status: PublicationStatus.Published);
            _pubRepo.Setup(r => r.AddAsync(It.IsAny<PublicationEntity>(), default)).Returns(Task.CompletedTask);
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            var result = await _sut.CreateAsync(publication);

            Assert.Equal(PublicationStatus.Draft, result.Status);
        }

        [Fact]
        public async Task CreateAsync_CallsAddAndSaveChanges()
        {
            var publication = BuildPublication();
            _pubRepo.Setup(r => r.AddAsync(publication, default)).Returns(Task.CompletedTask);
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            await _sut.CreateAsync(publication);

            _pubRepo.Verify(r => r.AddAsync(publication, default), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_DraftPublication_CallsUpdateAndSave()
        {
            var publication = BuildPublication(status: PublicationStatus.Draft, title: "Original");
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(publication));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            publication.Title = "Updated";

            var result = await _sut.UpdateAsync(publication);

            Assert.Equal("Updated", result.Title);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_RejectedPublication_IsAllowedToBeEdited()
        {
            var publication = BuildPublication(status: PublicationStatus.Rejected);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            await _sut.UpdateAsync(publication);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_SubmittedPublication_ThrowsInvalidOperationException()
        {
            var publication = BuildPublication(status: PublicationStatus.Submitted);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.UpdateAsync(publication));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_PublishedPublication_ThrowsInvalidOperationException()
        {
            var publication = BuildPublication(status: PublicationStatus.Published);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.UpdateAsync(publication));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_NonExistentPublication_ThrowsKeyNotFoundException()
        {
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.UpdateAsync(BuildPublication()));
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteAsync_ExistingPublication_CallsRemoveAndSave()
        {
            var publication = BuildPublication(type: PublicationType.ArticleJournal);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Remove(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            await _sut.DeleteAsync(publication.Id);

            _pubRepo.Verify(r => r.Remove(publication), Times.Once());
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_NonExistentId_ThrowsKeyNotFoundException()
        {
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.DeleteAsync(Guid.NewGuid()));

            _pubRepo.Verify(r => r.Remove(It.IsAny<PublicationEntity>()), Times.Never());
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never());
        }

        #endregion

        #region UpdateVisibilityAsync

        [Fact]
        public async Task UpdateVisibilityAsync_ExistingPublication_ChangesVisibilityAndSaves()
        {
            var publication = BuildPublication(visibility: PublicationVisibility.Public);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            var result = await _sut.UpdateVisibilityAsync(publication.Id, PublicationVisibility.Private);

            Assert.Equal(PublicationVisibility.Private, result.Visibility);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateVisibilityAsync_NonExistentPublication_ThrowsKeyNotFoundException()
        {
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.UpdateVisibilityAsync(Guid.NewGuid(), PublicationVisibility.Private));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        #endregion

        #region SubmitAsync

        [Fact]
        public async Task SubmitAsync_DraftPublication_TransitionsToSubmitted()
        {
            var publication = BuildPublication(status: PublicationStatus.Draft);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            var result = await _sut.SubmitAsync(publication.Id);

            Assert.Equal(PublicationStatus.Submitted, result.Status);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task SubmitAsync_NonDraftPublication_ThrowsInvalidOperationException()
        {
            var publication = BuildPublication(status: PublicationStatus.Submitted);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.SubmitAsync(publication.Id));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task SubmitAsync_PublishedPublication_ThrowsInvalidOperationException()
        {
            var publication = BuildPublication(status: PublicationStatus.Published);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.SubmitAsync(publication.Id));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task SubmitAsync_NonExistentPublication_ThrowsKeyNotFoundException()
        {
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.SubmitAsync(Guid.NewGuid()));
        }

        #endregion

        #region ApproveAsync

        [Fact]
        public async Task ApproveAsync_SubmittedPublication_TransitionsToPublished()
        {
            var publication = BuildPublication(status: PublicationStatus.Submitted);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            var result = await _sut.ApproveAsync(publication.Id);

            Assert.Equal(PublicationStatus.Published, result.Status);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task ApproveAsync_DraftPublication_ThrowsInvalidOperationException()
        {
            var publication = BuildPublication(status: PublicationStatus.Draft);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.ApproveAsync(publication.Id));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task ApproveAsync_AlreadyPublishedPublication_ThrowsInvalidOperationException()
        {
            var publication = BuildPublication(status: PublicationStatus.Published);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.ApproveAsync(publication.Id));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task ApproveAsync_NonExistentPublication_ThrowsKeyNotFoundException()
        {
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.ApproveAsync(Guid.NewGuid()));
        }

        #endregion

        #region RejectAsync

        [Fact]
        public async Task RejectAsync_SubmittedPublication_TransitionsToRejected()
        {
            var publication = BuildPublication(status: PublicationStatus.Submitted);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            var result = await _sut.RejectAsync(publication.Id, reason: "Insufficient references.");

            Assert.Equal(PublicationStatus.Rejected, result.Status);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task RejectAsync_DraftPublication_ThrowsInvalidOperationException()
        {
            var publication = BuildPublication(status: PublicationStatus.Draft);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.RejectAsync(publication.Id));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task RejectAsync_AlreadyRejectedPublication_ThrowsInvalidOperationException()
        {
            var publication = BuildPublication(status: PublicationStatus.Rejected);
            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.RejectAsync(publication.Id));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task RejectAsync_NonExistentPublication_ThrowsKeyNotFoundException()
        {
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.RejectAsync(Guid.NewGuid()));
        }

        #endregion

        #region PDF Management

        [Fact]
        public async Task AddPdfAsync_ExistingPublication_AppendsPdfUrlAndSaves()
        {
            const string newPdf = "https://storage/paper.pdf";
            var publication = BuildPublication();
            publication.AttachedPdfs = ["https://storage/existing.pdf"];

            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            var result = await _sut.AddPdfAsync(publication.Id, newPdf);

            Assert.Contains(newPdf, result.AttachedPdfs);
            Assert.Contains("https://storage/existing.pdf", result.AttachedPdfs);
            Assert.Equal(2, result.AttachedPdfs.Length);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task AddPdfAsync_NonExistentPublication_ThrowsKeyNotFoundException()
        {
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.AddPdfAsync(Guid.NewGuid(), "https://storage/paper.pdf"));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task RemovePdfAsync_ExistingPublication_RemovesMatchingUrlAndSaves()
        {
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

            var result = await _sut.RemovePdfAsync(publication.Id, target);

            Assert.DoesNotContain(target, result.AttachedPdfs);
            Assert.Contains(keep, result.AttachedPdfs);
            Assert.Single(result.AttachedPdfs);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task RemovePdfAsync_NonExistentPublication_ThrowsKeyNotFoundException()
        {
            _pubRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((PublicationEntity?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.RemovePdfAsync(Guid.NewGuid(), "https://storage/paper.pdf"));

            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task RemovePdfAsync_UrlNotInList_LeavesArrayUnchangedAndStillSaves()
        {
            const string keep = "https://storage/keep-me.pdf";
            var publication = BuildPublication();
            publication.AttachedPdfs = [keep];

            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default))
                .ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
            SetupGetByIdWithDetails(publication);

            var result = await _sut.RemovePdfAsync(publication.Id, "https://storage/ghost.pdf");

            Assert.Single(result.AttachedPdfs);
            Assert.Contains(keep, result.AttachedPdfs);
        }

        #endregion

        #region Statistics

        [Fact]
        public async Task CountPublicPublicationsAsync_DelegatesToRepoWithCorrectArguments()
        {
            _pubRepo
                .Setup(r => r.CountByStatusAndVisibilityAsync(
                    PublicationStatus.Published, PublicationVisibility.Public, default))
                .ReturnsAsync(17);

            var count = await _sut.CountPublicPublicationsAsync();

            Assert.Equal(17, count);
            _pubRepo.Verify(r => r.CountByStatusAndVisibilityAsync(
                PublicationStatus.Published, PublicationVisibility.Public, default), Times.Once);
        }

        [Fact]
        public async Task CountPublicPublicationsAsync_WhenNoneExist_ReturnsZero()
        {
            _pubRepo
                .Setup(r => r.CountByStatusAndVisibilityAsync(
                    PublicationStatus.Published, PublicationVisibility.Public, default))
                .ReturnsAsync(0);

            var count = await _sut.CountPublicPublicationsAsync();

            Assert.Equal(0, count);
        }

        #endregion
    }

    #endregion

    #region PublicationEntityExtensions

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

    #endregion
}
