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
    // Benefits over the EF in-memory approach:
    //   • No shared state between tests (each test creates its own mocks)
    //   • Tests run in microseconds (no DB round-trips at all)
    //   • You test ONLY the service logic, not EF or the repository
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
                CreatedAtUtc = DateTime.UtcNow,
            };

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
        // CREATE
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task CreateAsync_AlwaysSetsDraftStatus_RegardlessOfInput()
        {
            // Arrange — caller passes Published trying to bypass the workflow
            var publication = BuildPublication(status: PublicationStatus.Published);
            _pubRepo.Setup(r => r.AddAsync(It.IsAny<PublicationEntity>(), default)).Returns(Task.CompletedTask);
            _pubRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

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
            // We no longer need the JournalArticleEntity here if the service doesn't delete it

            _pubRepo
                .Setup(r => r.GetByIdAsync(publication.Id, default)) // Match the service's current implementation
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
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)) // Match the service's current implementation
                .ReturnsAsync((PublicationEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.DeleteAsync(Guid.NewGuid()));

            _pubRepo.Verify(r => r.Remove(It.IsAny<PublicationEntity>()), Times.Never());
            _pubRepo.Verify(r => r.SaveChangesAsync(default), Times.Never());
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