using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Storage;
using LIMTIC.Application.Contracts.Commands.Publications;
using LIMTIC.Application.Contracts.Queries.Publications;
using LIMTIC.Application.Mappings;
using LIMTIC.Application.Services.Publications;
using LIMTIC.Domain;
using LIMTIC.Domain.Abstractions.Publications;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;
using Moq;

namespace LIMTIC.UnitTests.Tests
{
    public class PublicationServiceTests
    {
        #region Mocks

        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IPublicationRepository> _pubRepo = new();
        private readonly Mock<IJournalArticleRepository> _journalRepo = new();
        private readonly Mock<ITechnicalReportRepository> _reportRepo = new();
        private readonly Mock<IBookChapterRepository> _chapterRepo = new();
        private readonly Mock<INationalConferenceRepository> _nationalRepo = new();
        private readonly Mock<IInternationalConferenceRepository> _intlRepo = new();
        private readonly Mock<IBlobStorageService> _blobStorage = new();
        private readonly Mock<ICurrentUserService> _currentUserService = new();

        #endregion

        #region System Under Test

        private readonly PublicationService _sut;

        public PublicationServiceTests()
        {
            // Default: authenticated user (non-null UserId) with no special role
            _currentUserService.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
            _currentUserService.SetupGet(x => x.Role).Returns((string?)null);

            _sut = new PublicationService(
                _unitOfWork.Object,
                _pubRepo.Object,
                _journalRepo.Object,
                _reportRepo.Object,
                _chapterRepo.Object,
                _nationalRepo.Object,
                _intlRepo.Object,
                _blobStorage.Object,
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
                Type = type,
                Status = status,
                Visibility = visibility,
                Year = year,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = createdAt ?? DateTime.UtcNow,
            };

        #endregion

        #region GetPublicationByIdAsync

        [Fact]
        public async Task GetPublicationByIdAsync_PublishedPublicPublication_ReturnsPublication()
        {
            // The service calls GetByIdWithDetailsAsync and only returns if status == Published
            var publication = BuildPublication(
                title: "My Publication",
                status: PublicationStatus.Published,
                visibility: PublicationVisibility.Public);

            _pubRepo
                .Setup(r => r.GetByIdWithDetailsAsync(publication.Id))
                .ReturnsAsync(publication);

            var result = await _sut.GetPublicationByIdAsync(publication.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(publication.Id, result.Data?.Id);
            Assert.Equal("My Publication", result.Data?.Title);
        }

        [Fact]
        public async Task GetPublicationByIdAsync_NonExistentId_ReturnsFailure()
        {
            _pubRepo
                .Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>()))
                .ReturnsAsync((PublicationEntity?)null);

            var result = await _sut.GetPublicationByIdAsync(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task GetPublicationByIdAsync_DraftPublication_ReturnsFailure()
        {
            // Service returns failure if status != Published
            var publication = BuildPublication(status: PublicationStatus.Draft);
            _pubRepo
                .Setup(r => r.GetByIdWithDetailsAsync(publication.Id))
                .ReturnsAsync(publication);

            var result = await _sut.GetPublicationByIdAsync(publication.Id);

            Assert.False(result.Success);
        }

        #endregion

        #region GetPublicationsAsync

        [Fact]
        public async Task GetPublicationsAsync_ForwardsTypeAndStatusToRepository()
        {
            var expected = (Items: (IEnumerable<PublicationEntity>)new List<PublicationEntity>(), TotalCount: 0);

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

            _pubRepo
                .Setup(r => r.GetFilteredAsync(
                    PublicationType.ArticleJournal,
                    PublicationStatus.Published,
                    It.IsAny<PublicationVisibility?>(),
                    null,
                    null,
                    null,
                    null,
                    1,
                    10))
                .ReturnsAsync(expected);

            var result = await _sut.GetPublicationsAsync(query);

            Assert.True(result.Success);
            _pubRepo.Verify(r => r.GetFilteredAsync(
                PublicationType.ArticleJournal,
                PublicationStatus.Published,
                It.IsAny<PublicationVisibility?>(),
                null,
                null,
                null,
                null,
                1,
                10), Times.Once);
        }

        [Fact]
        public async Task GetPublicationsAsync_ReturnsTotalCountFromRepository()
        {
            var items = new List<PublicationEntity> { BuildPublication(), BuildPublication() };
            var expected = (Items: (IEnumerable<PublicationEntity>)items, TotalCount: 42);

            _pubRepo
                .Setup(r => r.GetFilteredAsync(
                    It.IsAny<PublicationType?>(), It.IsAny<PublicationStatus?>(),
                    It.IsAny<PublicationVisibility?>(), It.IsAny<Guid?>(),
                    It.IsAny<Guid?>(), It.IsAny<int?>(), It.IsAny<string?>(),
                    It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expected);

            var result = await _sut.GetPublicationsAsync(new GetPublicationsQuery { Page = 1, Limit = 10 });

            Assert.True(result.Success);
            Assert.Equal(42, result.Data.Total);
            Assert.Equal(2, result.Data.Publications.Count);
        }

        [Fact]
        public async Task GetPublicationsAsync_UnauthenticatedUser_ForcesPublicVisibility()
        {
            // When UserId is null/empty, the service forces visibility = Public
            _currentUserService.SetupGet(x => x.UserId).Returns(Guid.Empty);

            var expected = (Items: (IEnumerable<PublicationEntity>)new List<PublicationEntity>(), TotalCount: 0);
            _pubRepo
                .Setup(r => r.GetFilteredAsync(
                    It.IsAny<PublicationType?>(), It.IsAny<PublicationStatus?>(),
                    PublicationVisibility.Public,
                    It.IsAny<Guid?>(), It.IsAny<Guid?>(),
                    It.IsAny<int?>(), It.IsAny<string?>(),
                    It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expected);

            await _sut.GetPublicationsAsync(new GetPublicationsQuery { Page = 1, Limit = 10 });

            _pubRepo.Verify(r => r.GetFilteredAsync(
                It.IsAny<PublicationType?>(), It.IsAny<PublicationStatus?>(),
                PublicationVisibility.Public,
                It.IsAny<Guid?>(), It.IsAny<Guid?>(),
                It.IsAny<int?>(), It.IsAny<string?>(),
                It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetPublicationsAsync_PageBelowOne_DefaultsToPageOne()
        {
            var expected = (Items: (IEnumerable<PublicationEntity>)new List<PublicationEntity>(), TotalCount: 0);
            _pubRepo
                .Setup(r => r.GetFilteredAsync(
                    It.IsAny<PublicationType?>(), It.IsAny<PublicationStatus?>(),
                    It.IsAny<PublicationVisibility?>(), It.IsAny<Guid?>(),
                    It.IsAny<Guid?>(), It.IsAny<int?>(), It.IsAny<string?>(),
                    1, It.IsAny<int>()))
                .ReturnsAsync(expected);

            await _sut.GetPublicationsAsync(new GetPublicationsQuery { Page = 0, Limit = 10 });

            _pubRepo.Verify(r => r.GetFilteredAsync(
                It.IsAny<PublicationType?>(), It.IsAny<PublicationStatus?>(),
                It.IsAny<PublicationVisibility?>(), It.IsAny<Guid?>(),
                It.IsAny<Guid?>(), It.IsAny<int?>(), It.IsAny<string?>(),
                1, It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region CreatePublicationAsync

        [Fact]
        public async Task CreatePublicationAsync_AuthenticatedUser_CallsAddAndCommits()
        {
            var publication = BuildPublication(type: PublicationType.ArticleJournal, status: PublicationStatus.Draft);

            _pubRepo.Setup(r => r.AddAsync(It.IsAny<PublicationEntity>())).Returns(Task.CompletedTask);
            _journalRepo.Setup(r => r.AddAsync(It.IsAny<JournalArticleEntity>())).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(2);
            _unitOfWork.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var command = new CreatePublicationCommand
            {
                Publication = publication.ToDto()
            };
            var result = await _sut.CreatePublicationAsync(command);

            Assert.True(result.Success);
            _pubRepo.Verify(r => r.AddAsync(It.IsAny<PublicationEntity>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreatePublicationAsync_UnauthenticatedUser_ReturnsFailure()
        {
            _currentUserService.SetupGet(x => x.UserId).Returns(Guid.Empty);

            var publication = BuildPublication();
            var command = new CreatePublicationCommand { Publication = publication.ToDto() };

            _unitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _sut.CreatePublicationAsync(command);

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.AddAsync(It.IsAny<PublicationEntity>()), Times.Never);
        }

        [Fact]
        public async Task CreatePublicationAsync_SaveChangesReturnsUnexpectedCount_RollsBackAndReturnsFailure()
        {
            var publication = BuildPublication(type: PublicationType.ArticleJournal);

            _pubRepo.Setup(r => r.AddAsync(It.IsAny<PublicationEntity>())).Returns(Task.CompletedTask);
            _journalRepo.Setup(r => r.AddAsync(It.IsAny<JournalArticleEntity>())).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(0); // unexpected
            _unitOfWork.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _sut.CreatePublicationAsync(new CreatePublicationCommand { Publication = publication.ToDto() });

            Assert.False(result.Success);
            _unitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
            _unitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Never);
        }

        #endregion

        #region UpdatePublicationAsync

        [Fact]
        public async Task UpdatePublicationAsync_DraftPublicationByOwner_CallsUpdateAndSave()
        {
            var ownerId = Guid.NewGuid();
            _currentUserService.SetupGet(x => x.UserId).Returns(ownerId);

            var publication = BuildPublication(status: PublicationStatus.Draft, userId: ownerId, title: "Original");
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            publication.Title = "Updated";
            var command = new UpdatePublicationCommand { Publication = publication.ToDto() };
            var result = await _sut.UpdatePublicationAsync(command);

            Assert.True(result.Success);
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdatePublicationAsync_RejectedPublicationByOwner_IsAllowedToBeEdited()
        {
            var ownerId = Guid.NewGuid();
            _currentUserService.SetupGet(x => x.UserId).Returns(ownerId);

            var publication = BuildPublication(status: PublicationStatus.Rejected, userId: ownerId);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var command = new UpdatePublicationCommand { Publication = publication.ToDto() };
            var result = await _sut.UpdatePublicationAsync(command);

            Assert.True(result.Success);
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePublicationAsync_SubmittedPublicationByNonAdmin_ReturnsFailure()
        {
            var ownerId = Guid.NewGuid();
            _currentUserService.SetupGet(x => x.UserId).Returns(ownerId);
            _currentUserService.SetupGet(x => x.Role).Returns("User");

            var publication = BuildPublication(status: PublicationStatus.Submitted, userId: ownerId);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);

            var command = new UpdatePublicationCommand { Publication = publication.ToDto() };
            var result = await _sut.UpdatePublicationAsync(command);

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePublicationAsync_PublishedPublicationByNonAdmin_ReturnsFailure()
        {
            var ownerId = Guid.NewGuid();
            _currentUserService.SetupGet(x => x.UserId).Returns(ownerId);
            _currentUserService.SetupGet(x => x.Role).Returns("User");

            var publication = BuildPublication(status: PublicationStatus.Published, userId: ownerId);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);

            var command = new UpdatePublicationCommand { Publication = publication.ToDto() };
            var result = await _sut.UpdatePublicationAsync(command);

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePublicationAsync_NonExistentPublication_ReturnsFailure()
        {
            _pubRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PublicationEntity?)null);

            var publication = BuildPublication();
            var command = new UpdatePublicationCommand { Publication = publication.ToDto() };
            var result = await _sut.UpdatePublicationAsync(command);

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePublicationAsync_NonOwnerNonAdmin_ReturnsFailure()
        {
            _currentUserService.SetupGet(x => x.UserId).Returns(Guid.NewGuid()); // different from publication's owner
            _currentUserService.SetupGet(x => x.Role).Returns("User");

            var publication = BuildPublication(status: PublicationStatus.Draft, userId: Guid.NewGuid());
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);

            var command = new UpdatePublicationCommand { Publication = publication.ToDto() };
            var result = await _sut.UpdatePublicationAsync(command);

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
        }

        #endregion

        #region DeletePublicationAsync

        [Fact]
        public async Task DeletePublicationAsync_DraftByOwner_CallsRemoveAndSave()
        {
            var ownerId = Guid.NewGuid();
            _currentUserService.SetupGet(x => x.UserId).Returns(ownerId);

            var publication = BuildPublication(status: PublicationStatus.Draft, userId: ownerId);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Remove(It.IsAny<PublicationEntity>()));
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _sut.DeletePublicationAsync(publication.Id);

            Assert.True(result.Success);
            _pubRepo.Verify(r => r.Remove(publication), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeletePublicationAsync_NonExistentId_ReturnsFailure()
        {
            _pubRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PublicationEntity?)null);

            var result = await _sut.DeletePublicationAsync(Guid.NewGuid());

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.Remove(It.IsAny<PublicationEntity>()), Times.Never);
        }

        [Fact]
        public async Task DeletePublicationAsync_NonDraftByNonAdmin_ReturnsFailure()
        {
            var ownerId = Guid.NewGuid();
            _currentUserService.SetupGet(x => x.UserId).Returns(ownerId);
            _currentUserService.SetupGet(x => x.Role).Returns("User");

            var publication = BuildPublication(status: PublicationStatus.Published, userId: ownerId);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);

            var result = await _sut.DeletePublicationAsync(publication.Id);

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.Remove(It.IsAny<PublicationEntity>()), Times.Never);
        }

        [Fact]
        public async Task DeletePublicationAsync_AdminCanDeletePublishedPublication()
        {
            _currentUserService.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
            _currentUserService.SetupGet(x => x.Role).Returns("Admin");

            var publication = BuildPublication(status: PublicationStatus.Published);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Remove(It.IsAny<PublicationEntity>()));
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _sut.DeletePublicationAsync(publication.Id);

            Assert.True(result.Success);
            _pubRepo.Verify(r => r.Remove(publication), Times.Once);
        }

        #endregion

        #region SubmitPublicationAsync

        [Fact]
        public async Task SubmitPublicationAsync_DraftByOwner_TransitionsToSubmitted()
        {
            var ownerId = Guid.NewGuid();
            _currentUserService.SetupGet(x => x.UserId).Returns(ownerId);

            var publication = BuildPublication(status: PublicationStatus.Draft, userId: ownerId);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _sut.SubmitPublicationAsync(publication.Id);

            Assert.True(result.Success);
            Assert.Equal(PublicationStatus.Submitted, publication.Status);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
        }

        [Fact]
        public async Task SubmitPublicationAsync_RejectedByOwner_TransitionsToSubmitted()
        {
            var ownerId = Guid.NewGuid();
            _currentUserService.SetupGet(x => x.UserId).Returns(ownerId);

            var publication = BuildPublication(status: PublicationStatus.Rejected, userId: ownerId);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _sut.SubmitPublicationAsync(publication.Id);

            Assert.True(result.Success);
            Assert.Equal(PublicationStatus.Submitted, publication.Status);
        }

        [Fact]
        public async Task SubmitPublicationAsync_AlreadySubmittedPublication_ReturnsFailure()
        {
            var ownerId = Guid.NewGuid();
            _currentUserService.SetupGet(x => x.UserId).Returns(ownerId);

            var publication = BuildPublication(status: PublicationStatus.Submitted, userId: ownerId);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);

            var result = await _sut.SubmitPublicationAsync(publication.Id);

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
        }

        [Fact]
        public async Task SubmitPublicationAsync_PublishedPublication_ReturnsFailure()
        {
            var ownerId = Guid.NewGuid();
            _currentUserService.SetupGet(x => x.UserId).Returns(ownerId);

            var publication = BuildPublication(status: PublicationStatus.Published, userId: ownerId);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);

            var result = await _sut.SubmitPublicationAsync(publication.Id);

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
        }

        [Fact]
        public async Task SubmitPublicationAsync_NonExistentPublication_ReturnsFailure()
        {
            _currentUserService.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
            _pubRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PublicationEntity?)null);

            var result = await _sut.SubmitPublicationAsync(Guid.NewGuid());

            Assert.False(result.Success);
        }

        [Fact]
        public async Task SubmitPublicationAsync_NonOwner_ReturnsFailure()
        {
            _currentUserService.SetupGet(x => x.UserId).Returns(Guid.NewGuid()); // not the owner

            var publication = BuildPublication(status: PublicationStatus.Draft, userId: Guid.NewGuid());
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);

            var result = await _sut.SubmitPublicationAsync(publication.Id);

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
        }

        #endregion

        #region ValidatePublicationAsync (Approve)

        [Fact]
        public async Task ValidatePublicationAsync_SubmittedByAdmin_TransitionsToPublished()
        {
            _currentUserService.SetupGet(x => x.Role).Returns("Admin");

            var publication = BuildPublication(status: PublicationStatus.Submitted);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _sut.ValidatePublicationAsync(publication.Id);

            Assert.True(result.Success);
            Assert.Equal(PublicationStatus.Published, publication.Status);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
        }

        [Fact]
        public async Task ValidatePublicationAsync_DraftPublication_ReturnsFailure()
        {
            _currentUserService.SetupGet(x => x.Role).Returns("Admin");

            var publication = BuildPublication(status: PublicationStatus.Draft);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);

            var result = await _sut.ValidatePublicationAsync(publication.Id);

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
        }

        [Fact]
        public async Task ValidatePublicationAsync_AlreadyPublished_ReturnsFailure()
        {
            _currentUserService.SetupGet(x => x.Role).Returns("Admin");

            var publication = BuildPublication(status: PublicationStatus.Published);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);

            var result = await _sut.ValidatePublicationAsync(publication.Id);

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
        }

        [Fact]
        public async Task ValidatePublicationAsync_NonAdminUser_ReturnsFailure()
        {
            _currentUserService.SetupGet(x => x.Role).Returns("User");

            var publication = BuildPublication(status: PublicationStatus.Submitted);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);

            var result = await _sut.ValidatePublicationAsync(publication.Id);

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
        }

        [Fact]
        public async Task ValidatePublicationAsync_NonExistentPublication_ReturnsFailure()
        {
            _currentUserService.SetupGet(x => x.Role).Returns("Admin");
            _pubRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PublicationEntity?)null);

            var result = await _sut.ValidatePublicationAsync(Guid.NewGuid());

            Assert.False(result.Success);
        }

        #endregion

        #region RejectPublicationAsync

        [Fact]
        public async Task RejectPublicationAsync_SubmittedByAdmin_TransitionsToRejected()
        {
            _currentUserService.SetupGet(x => x.Role).Returns("Admin");

            var publication = BuildPublication(status: PublicationStatus.Submitted);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);
            _pubRepo.Setup(r => r.Update(It.IsAny<PublicationEntity>()));
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _sut.RejectPublicationAsync(publication.Id);

            Assert.True(result.Success);
            Assert.Equal(PublicationStatus.Rejected, publication.Status);
            _pubRepo.Verify(r => r.Update(publication), Times.Once);
        }

        [Fact]
        public async Task RejectPublicationAsync_DraftPublication_ReturnsFailure()
        {
            _currentUserService.SetupGet(x => x.Role).Returns("Admin");

            var publication = BuildPublication(status: PublicationStatus.Draft);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);

            var result = await _sut.RejectPublicationAsync(publication.Id);

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
        }

        [Fact]
        public async Task RejectPublicationAsync_AlreadyRejected_ReturnsFailure()
        {
            _currentUserService.SetupGet(x => x.Role).Returns("Admin");

            var publication = BuildPublication(status: PublicationStatus.Rejected);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);

            var result = await _sut.RejectPublicationAsync(publication.Id);

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
        }

        [Fact]
        public async Task RejectPublicationAsync_NonAdminUser_ReturnsFailure()
        {
            _currentUserService.SetupGet(x => x.Role).Returns("User");

            var publication = BuildPublication(status: PublicationStatus.Submitted);
            _pubRepo.Setup(r => r.GetByIdAsync(publication.Id)).ReturnsAsync(publication);

            var result = await _sut.RejectPublicationAsync(publication.Id);

            Assert.False(result.Success);
            _pubRepo.Verify(r => r.Update(It.IsAny<PublicationEntity>()), Times.Never);
        }

        [Fact]
        public async Task RejectPublicationAsync_NonExistentPublication_ReturnsFailure()
        {
            _currentUserService.SetupGet(x => x.Role).Returns("Admin");
            _pubRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PublicationEntity?)null);

            var result = await _sut.RejectPublicationAsync(Guid.NewGuid());

            Assert.False(result.Success);
        }

        #endregion
    }
}