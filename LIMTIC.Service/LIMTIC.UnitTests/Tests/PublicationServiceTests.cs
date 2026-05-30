using LIMTIC.Application.Contracts.Commands.Publications;
using LIMTIC.Application.Contracts.Queries.Publications;
using LIMTIC.Application.DTOs.Publications;
using LIMTIC.Application.Mappings;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;
using LIMTIC.UnitTests.Base;

namespace LIMTIC.UnitTests.Tests
{
    /// <summary>
    /// Integration-style unit tests for <see cref="PublicationService"/>.
    /// Uses real service / repository instances wired up by <see cref="BaseTests"/>
    /// and an in-memory EF Core database — no Moq anywhere.
    /// </summary>
    public class PublicationServiceTests : BaseTests
    {
        // ── Builder helper ─────────────────────────────────────────────────────

        private static PublicationEntity BuildPublication(
            PublicationType type = PublicationType.ArticleJournal,
            PublicationStatus status = PublicationStatus.Draft,
            PublicationVisibility visibility = PublicationVisibility.Public,
            int year = 2024,
            Guid? userId = null,
            Guid? researchAxisId = null,
            string title = "Test Publication",
            DateTime? createdAt = null) => new PublicationEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                ResearchAxisId = researchAxisId ?? Guid.NewGuid(),
                Title = title,
                Abstract = "Test abstract.",
                Keywords = ["AI", "ML"],
                Authors = ["Author One", "Author Two"],
                Type = type,
                Status = status,
                Visibility = visibility,
                Year = year,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = createdAt ?? DateTime.UtcNow,
            };

        private PublicationDto BuildJournalArticleDto(
    Guid? userId = null,
    Guid? researchAxisId = null,
    PublicationStatus status = PublicationStatus.Draft,
    PublicationVisibility visibility = PublicationVisibility.Public)
        {
            return new PublicationDto
            {
                UserId = userId ?? CurrentUserService.UserId.Value,
                ResearchAxisId = researchAxisId ?? Guid.NewGuid(),
                Title = "Journal Article Test",
                Abstract = "Test abstract.",
                Keywords = ["AI", "ML"],
                Authors = ["Author One", "Author Two"],
                Doi = "10.1000/xyz123",
                Venue = "IEEE Transactions",
                Year = 2024,
                Type = PublicationType.ArticleJournal,
                Status = status,
                Visibility = visibility,
                JournalArticle = new JournalArticleDto
                {
                    JournalName = "IEEE Transactions on Neural Networks",
                    Volume = "35",
                    Number = "4",
                    Pages = 12,
                    Ranking = JournalRanking.Q1
                }
            };
        }

        private PublicationDto BuildTechnicalReportDto(
            Guid? userId = null,
            Guid? researchAxisId = null,
            PublicationStatus status = PublicationStatus.Draft,
            PublicationVisibility visibility = PublicationVisibility.Public)
        {
            return new PublicationDto
            {
                UserId = userId ?? CurrentUserService.UserId.Value,
                ResearchAxisId = researchAxisId ?? Guid.NewGuid(),
                Title = "Technical Report Test",
                Abstract = "Test abstract.",
                Keywords = ["Systems", "Performance"],
                Authors = ["Author One", "Author Two"],
                Doi = null,
                Venue = null,
                Year = 2024,
                Type = PublicationType.TechnicalReport,
                Status = status,
                Visibility = visibility,
                TechnicalReport = new TechnicalReportDto
                {
                    ReportNumber = 2024001,
                    Institution = "LIMTIC Research Lab"
                }
            };
        }

        private PublicationDto BuildBookChapterDto(
            Guid? userId = null,
            Guid? researchAxisId = null,
            PublicationStatus status = PublicationStatus.Draft,
            PublicationVisibility visibility = PublicationVisibility.Public)
        {
            return new PublicationDto
            {
                UserId = userId ?? CurrentUserService.UserId.Value,
                ResearchAxisId = researchAxisId ?? Guid.NewGuid(),
                Title = "Book Chapter Test",
                Abstract = "Test abstract.",
                Keywords = ["Deep Learning", "NLP"],
                Authors = ["Author One", "Author Two"],
                Doi = "10.1007/978-3-030-00001-1_5",
                Venue = null,
                Year = 2024,
                Type = PublicationType.BookChapter,
                Status = status,
                Visibility = visibility,
                BookChapter = new BookChapterDto
                {
                    BookTitle = "Advances in Artificial Intelligence",
                    Publisher = "Springer",
                    Isbn = "978-3-030-00001-1",
                    Pages = "45-67"
                }
            };
        }

        private PublicationDto BuildNationalConferenceDto(
            Guid? userId = null,
            Guid? researchAxisId = null,
            PublicationStatus status = PublicationStatus.Draft,
            PublicationVisibility visibility = PublicationVisibility.Public)
        {
            return new PublicationDto
            {
                UserId = userId ?? CurrentUserService.UserId.Value,
                ResearchAxisId = researchAxisId ?? Guid.NewGuid(),
                Title = "National Conference Paper Test",
                Abstract = "Test abstract.",
                Keywords = ["Computer Vision", "Recognition"],
                Authors = ["Author One", "Author Two"],
                Doi = null,
                Venue = "Algiers",
                Year = 2024,
                Type = PublicationType.NationalConference,
                Status = status,
                Visibility = visibility,
                NationalConference = new NationalConferenceDto
                {
                    ConferenceName = "Conférence Nationale sur l'Informatique",
                    Location = "Algiers, Algeria"
                }
            };
        }

        private PublicationDto BuildInternationalConferenceDto(
            Guid? userId = null,
            Guid? researchAxisId = null,
            PublicationStatus status = PublicationStatus.Draft,
            PublicationVisibility visibility = PublicationVisibility.Public)
        {
            return new PublicationDto
            {
                UserId = userId ?? CurrentUserService.UserId.Value,
                ResearchAxisId = researchAxisId ?? Guid.NewGuid(),
                Title = "International Conference Paper Test",
                Abstract = "Test abstract.",
                Keywords = ["Reinforcement Learning", "Robotics"],
                Authors = ["Author One", "Author Two"],
                Doi = "10.1145/3411764.3445999",
                Venue = "NeurIPS 2024",
                Year = 2024,
                Type = PublicationType.InternationalConference,
                Status = status,
                Visibility = visibility,
                InternationalConference = new InternationalConferenceDto
                {
                    ConferenceName = "Neural Information Processing Systems",
                    Location = "Vancouver, Canada",
                    Ranking = CoreRanking.APlus
                }
            };
        }

        /// <summary>
        /// Persists a <see cref="PublicationEntity"/> directly through the repository
        /// so tests that exercise read/update/delete paths start with known data.
        /// </summary>
        private async Task SeedPublicationAsync(PublicationEntity publication)
        {
            await PublicationRepository.AddAsync(publication);
            bool result = await UnitOfWork.SaveChangesAsync() > 0;
            Assert.True(result);
        }

        // ── GetPublicationByIdAsync ────────────────────────────────────────────

        [Fact]
        public async Task GetPublicationByIdAsync_PublishedPublicPublication_ReturnsPublication()
        {
            var publication = BuildPublication(
                title: "My Publication",
                status: PublicationStatus.Published,
                visibility: PublicationVisibility.Public);

            await SeedPublicationAsync(publication);

            var result = await PublicationService.GetPublicationByIdAsync(publication.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(publication.Id, result.Data?.Id);
            Assert.Equal("My Publication", result.Data?.Title);
        }

        [Fact]
        public async Task GetPublicationByIdAsync_NonExistentId_ReturnsFailure()
        {
            var result = await PublicationService.GetPublicationByIdAsync(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task GetPublicationByIdAsync_DraftPublication_ReturnsFailure()
        {
            var publication = BuildPublication(status: PublicationStatus.Draft);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.GetPublicationByIdAsync(publication.Id);

            Assert.False(result.Success);
        }

        // ── GetPublicationsAsync ───────────────────────────────────────────────

        [Fact]
        public async Task GetPublicationsAsync_ForwardsTypeAndStatusToRepository()
        {
            // Seed one matching and one non-matching publication
            var matching = BuildPublication(
                type: PublicationType.ArticleJournal,
                status: PublicationStatus.Published);
            var other = BuildPublication(
                type: PublicationType.TechnicalReport,
                status: PublicationStatus.Draft);

            await SeedPublicationAsync(matching);
            await SeedPublicationAsync(other);

            var query = new GetPublicationsQuery
            {
                Type = PublicationType.ArticleJournal.ToString(),
                Status = PublicationStatus.Published.ToString(),
                Visibility = null,
                UserId = null,
                AxeId = null,
                Year = null,
                Search = null,
                Page = 1,
                Limit = 10
            };

            var result = await PublicationService.GetPublicationsAsync(query);

            Assert.True(result.Success);
            Assert.All(result.Data.Publications,
                p => Assert.Equal(PublicationType.ArticleJournal, p.Type));
            Assert.All(result.Data.Publications,
                p => Assert.Equal(PublicationStatus.Published, p.Status));
        }

        [Fact]
        public async Task GetPublicationsAsync_ReturnsTotalCountFromRepository()
        {
            // Seed 2 published public articles
            await SeedPublicationAsync(BuildPublication(
                status: PublicationStatus.Published, visibility: PublicationVisibility.Public));
            await SeedPublicationAsync(BuildPublication(
                status: PublicationStatus.Published, visibility: PublicationVisibility.Public));

            var result = await PublicationService.GetPublicationsAsync(
                new GetPublicationsQuery { Page = 1, Limit = 10 });

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Publications.Count);
            Assert.Equal(2, result.Data.Total);
        }

        [Fact]
        public async Task GetPublicationsAsync_UnauthenticatedUser_ForcesPublicVisibility()
        {
            // Act as an unauthenticated user
            CurrentUserService.UserId = Guid.Empty;

            var privatePublication = BuildPublication(
                status: PublicationStatus.Published,
                visibility: PublicationVisibility.Private);
            var publicPublication = BuildPublication(
                status: PublicationStatus.Published,
                visibility: PublicationVisibility.Public);

            await SeedPublicationAsync(privatePublication);
            await SeedPublicationAsync(publicPublication);

            var result = await PublicationService.GetPublicationsAsync(
                new GetPublicationsQuery { Page = 1, Limit = 10 });

            Assert.True(result.Success);
            // Only the public one should be returned
            Assert.All(result.Data.Publications,
                p => Assert.Equal(PublicationVisibility.Public, p.Visibility));
        }

        [Fact]
        public async Task GetPublicationsAsync_PageBelowOne_DefaultsToPageOne()
        {
            await SeedPublicationAsync(BuildPublication(
                status: PublicationStatus.Published, visibility: PublicationVisibility.Public));

            // Page = 0 should silently be treated as page 1 — no exception, data returned
            var result = await PublicationService.GetPublicationsAsync(
                new GetPublicationsQuery { Page = 0, Limit = 10 });

            Assert.True(result.Success);
            Assert.NotEmpty(result.Data.Publications);
        }

        // ── CreatePublicationAsync ─────────────────────────────────────────────

        [Fact]
        public async Task CreatePublicationAsync_AuthenticatedUser_CallsAddAndCommits()
        {
            var ownerId = Guid.NewGuid();
            CurrentUserService.UserId = ownerId;

            var publication = BuildJournalArticleDto(
                status: PublicationStatus.Draft,
                userId: ownerId);

            var result = await PublicationService.CreatePublicationAsync(
                new CreatePublicationCommand { Publication = publication });

            Assert.True(result.Success);

            // Verify the entity was persisted
            var saved = await PublicationRepository.GetByIdAsync(result.Data!.Id.Value);
            Assert.NotNull(saved);
        }

        [Fact]
        public async Task CreatePublicationAsync_UnauthenticatedUser_ReturnsFailure()
        {
            CurrentUserService.UserId = Guid.Empty;

            var publication = BuildPublication();
            var result = await PublicationService.CreatePublicationAsync(
                new CreatePublicationCommand { Publication = publication.ToDto() });

            Assert.False(result.Success);
        }

        // ── UpdatePublicationAsync ─────────────────────────────────────────────

        [Fact]
        public async Task UpdatePublicationAsync_DraftPublicationByOwner_CallsUpdateAndSave()
        {
            var ownerId = Guid.NewGuid();
            CurrentUserService.UserId = ownerId;

            var publication = BuildPublication(
                status: PublicationStatus.Draft,
                userId: ownerId,
                title: "Original");
            await SeedPublicationAsync(publication);

            publication.Title = "Updated";
            var result = await PublicationService.UpdatePublicationAsync(
                new UpdatePublicationCommand { Publication = publication.ToDto() });

            Assert.True(result.Success);

            var updated = await PublicationRepository.GetByIdAsync(publication.Id);
            Assert.Equal("Updated", updated?.Title);
        }

        [Fact]
        public async Task UpdatePublicationAsync_RejectedPublicationByOwner_IsAllowedToBeEdited()
        {
            var ownerId = Guid.NewGuid();
            CurrentUserService.UserId = ownerId;

            var publication = BuildPublication(
                status: PublicationStatus.Rejected,
                userId: ownerId);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.UpdatePublicationAsync(
                new UpdatePublicationCommand { Publication = publication.ToDto() });

            Assert.True(result.Success);
        }

        [Fact]
        public async Task UpdatePublicationAsync_SubmittedPublicationByNonAdmin_ReturnsFailure()
        {
            var ownerId = Guid.NewGuid();
            CurrentUserService.UserId = ownerId;
            CurrentUserService.Role = "User";

            var publication = BuildPublication(
                status: PublicationStatus.Submitted,
                userId: ownerId);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.UpdatePublicationAsync(
                new UpdatePublicationCommand { Publication = publication.ToDto() });

            Assert.False(result.Success);
        }

        [Fact]
        public async Task UpdatePublicationAsync_PublishedPublicationByNonAdmin_ReturnsFailure()
        {
            var ownerId = Guid.NewGuid();
            CurrentUserService.UserId = ownerId;
            CurrentUserService.Role = "User";

            var publication = BuildPublication(
                status: PublicationStatus.Published,
                userId: ownerId);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.UpdatePublicationAsync(
                new UpdatePublicationCommand { Publication = publication.ToDto() });

            Assert.False(result.Success);
        }

        [Fact]
        public async Task UpdatePublicationAsync_NonExistentPublication_ReturnsFailure()
        {
            var publication = BuildPublication();
            // intentionally NOT seeded

            var result = await PublicationService.UpdatePublicationAsync(
                new UpdatePublicationCommand { Publication = publication.ToDto() });

            Assert.False(result.Success);
        }

        [Fact]
        public async Task UpdatePublicationAsync_NonOwnerNonAdmin_ReturnsFailure()
        {
            CurrentUserService.UserId = Guid.NewGuid(); // someone else
            CurrentUserService.Role = "User";

            var publication = BuildPublication(
                status: PublicationStatus.Draft,
                userId: Guid.NewGuid()); // owned by yet another user
            await SeedPublicationAsync(publication);

            var result = await PublicationService.UpdatePublicationAsync(
                new UpdatePublicationCommand { Publication = publication.ToDto() });

            Assert.False(result.Success);
        }

        // ── DeletePublicationAsync ─────────────────────────────────────────────

        [Fact]
        public async Task DeletePublicationAsync_DraftByOwner_CallsRemoveAndSave()
        {
            var ownerId = Guid.NewGuid();
            CurrentUserService.UserId = ownerId;

            var publication = BuildPublication(
                status: PublicationStatus.Draft,
                userId: ownerId);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.DeletePublicationAsync(publication.Id);

            Assert.True(result.Success);

            var deleted = await PublicationRepository.GetByIdAsync(publication.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeletePublicationAsync_NonExistentId_ReturnsFailure()
        {
            var result = await PublicationService.DeletePublicationAsync(Guid.NewGuid());

            Assert.False(result.Success);
        }

        [Fact]
        public async Task DeletePublicationAsync_NonDraftByNonAdmin_ReturnsFailure()
        {
            var ownerId = Guid.NewGuid();
            CurrentUserService.UserId = ownerId;
            CurrentUserService.Role = "User";

            var publication = BuildPublication(
                status: PublicationStatus.Published,
                userId: ownerId);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.DeletePublicationAsync(publication.Id);

            Assert.False(result.Success);

            var stillExists = await PublicationRepository.GetByIdAsync(publication.Id);
            Assert.NotNull(stillExists);
        }

        [Fact]
        public async Task DeletePublicationAsync_AdminCanDeletePublishedPublication()
        {
            CurrentUserService.UserId = Guid.NewGuid();
            CurrentUserService.Role = "Admin";

            var publication = BuildPublication(status: PublicationStatus.Published);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.DeletePublicationAsync(publication.Id);

            Assert.True(result.Success);

            var deleted = await PublicationRepository.GetByIdAsync(publication.Id);
            Assert.Null(deleted);
        }

        // ── SubmitPublicationAsync ─────────────────────────────────────────────

        [Fact]
        public async Task SubmitPublicationAsync_DraftByOwner_TransitionsToSubmitted()
        {
            var ownerId = Guid.NewGuid();
            CurrentUserService.UserId = ownerId;

            var publication = BuildPublication(
                status: PublicationStatus.Draft,
                userId: ownerId);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.SubmitPublicationAsync(publication.Id);

            Assert.True(result.Success);

            var updated = await PublicationRepository.GetByIdAsync(publication.Id);
            Assert.Equal(PublicationStatus.Submitted, updated?.Status);
        }

        [Fact]
        public async Task SubmitPublicationAsync_RejectedByOwner_TransitionsToSubmitted()
        {
            var ownerId = Guid.NewGuid();
            CurrentUserService.UserId = ownerId;

            var publication = BuildPublication(
                status: PublicationStatus.Rejected,
                userId: ownerId);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.SubmitPublicationAsync(publication.Id);

            Assert.True(result.Success);

            var updated = await PublicationRepository.GetByIdAsync(publication.Id);
            Assert.Equal(PublicationStatus.Submitted, updated?.Status);
        }

        [Fact]
        public async Task SubmitPublicationAsync_AlreadySubmittedPublication_ReturnsFailure()
        {
            var ownerId = Guid.NewGuid();
            CurrentUserService.UserId = ownerId;

            var publication = BuildPublication(
                status: PublicationStatus.Submitted,
                userId: ownerId);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.SubmitPublicationAsync(publication.Id);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task SubmitPublicationAsync_PublishedPublication_ReturnsFailure()
        {
            var ownerId = Guid.NewGuid();
            CurrentUserService.UserId = ownerId;

            var publication = BuildPublication(
                status: PublicationStatus.Published,
                userId: ownerId);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.SubmitPublicationAsync(publication.Id);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task SubmitPublicationAsync_NonExistentPublication_ReturnsFailure()
        {
            CurrentUserService.UserId = Guid.NewGuid();

            var result = await PublicationService.SubmitPublicationAsync(Guid.NewGuid());

            Assert.False(result.Success);
        }

        [Fact]
        public async Task SubmitPublicationAsync_NonOwner_ReturnsFailure()
        {
            CurrentUserService.UserId = Guid.NewGuid(); // not the owner

            var publication = BuildPublication(
                status: PublicationStatus.Draft,
                userId: Guid.NewGuid()); // different owner
            await SeedPublicationAsync(publication);

            var result = await PublicationService.SubmitPublicationAsync(publication.Id);

            Assert.False(result.Success);
        }

        // ── ValidatePublicationAsync (Approve) ─────────────────────────────────

        [Fact]
        public async Task ValidatePublicationAsync_SubmittedByAdmin_TransitionsToPublished()
        {
            CurrentUserService.Role = "Admin";

            var publication = BuildPublication(status: PublicationStatus.Submitted);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.ValidatePublicationAsync(publication.Id);

            Assert.True(result.Success);

            var updated = await PublicationRepository.GetByIdAsync(publication.Id);
            Assert.Equal(PublicationStatus.Published, updated?.Status);
        }

        [Fact]
        public async Task ValidatePublicationAsync_DraftPublication_ReturnsFailure()
        {
            CurrentUserService.Role = "Admin";

            var publication = BuildPublication(status: PublicationStatus.Draft);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.ValidatePublicationAsync(publication.Id);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task ValidatePublicationAsync_AlreadyPublished_ReturnsFailure()
        {
            CurrentUserService.Role = "Admin";

            var publication = BuildPublication(status: PublicationStatus.Published);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.ValidatePublicationAsync(publication.Id);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task ValidatePublicationAsync_NonAdminUser_ReturnsFailure()
        {
            CurrentUserService.Role = "User";

            var publication = BuildPublication(status: PublicationStatus.Submitted);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.ValidatePublicationAsync(publication.Id);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task ValidatePublicationAsync_NonExistentPublication_ReturnsFailure()
        {
            CurrentUserService.Role = "Admin";

            var result = await PublicationService.ValidatePublicationAsync(Guid.NewGuid());

            Assert.False(result.Success);
        }

        // ── RejectPublicationAsync ─────────────────────────────────────────────

        [Fact]
        public async Task RejectPublicationAsync_SubmittedByAdmin_TransitionsToRejected()
        {
            CurrentUserService.Role = "Admin";

            var publication = BuildPublication(status: PublicationStatus.Submitted);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.RejectPublicationAsync(publication.Id);

            Assert.True(result.Success);

            var updated = await PublicationRepository.GetByIdAsync(publication.Id);
            Assert.Equal(PublicationStatus.Rejected, updated?.Status);
        }

        [Fact]
        public async Task RejectPublicationAsync_DraftPublication_ReturnsFailure()
        {
            CurrentUserService.Role = "Admin";

            var publication = BuildPublication(status: PublicationStatus.Draft);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.RejectPublicationAsync(publication.Id);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task RejectPublicationAsync_AlreadyRejected_ReturnsFailure()
        {
            CurrentUserService.Role = "Admin";

            var publication = BuildPublication(status: PublicationStatus.Rejected);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.RejectPublicationAsync(publication.Id);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task RejectPublicationAsync_NonAdminUser_ReturnsFailure()
        {
            CurrentUserService.Role = "User";

            var publication = BuildPublication(status: PublicationStatus.Submitted);
            await SeedPublicationAsync(publication);

            var result = await PublicationService.RejectPublicationAsync(publication.Id);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task RejectPublicationAsync_NonExistentPublication_ReturnsFailure()
        {
            CurrentUserService.Role = "Admin";

            var result = await PublicationService.RejectPublicationAsync(Guid.NewGuid());

            Assert.False(result.Success);
        }
    }
}