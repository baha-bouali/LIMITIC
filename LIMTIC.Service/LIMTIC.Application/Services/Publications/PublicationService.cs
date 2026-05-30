using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.Abstractions.Storage;
using LIMTIC.Application.Contracts.Commands.Publications;
using LIMTIC.Application.Contracts.Queries.Publications;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Publications;
using LIMTIC.Application.DTOs.Storage;
using LIMTIC.Application.Mappings;
using LIMTIC.Domain;
using LIMTIC.Domain.Abstractions.Files;
using LIMTIC.Domain.Abstractions.Publications;
using LIMTIC.Domain.Entities.Files;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Services.Publications
{
    public class PublicationService : IPublicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublicationRepository _publicationRepository;
        private readonly IJournalArticleRepository _journalArticleRepository;
        private readonly ITechnicalReportRepository _technicalReportRepository;
        private readonly IBookChapterRepository _bookChapterRepository;
        private readonly INationalConferenceRepository _nationalConferenceRepository;
        private readonly IInternationalConferenceRepository _internationalConferenceRepository;
        private readonly IPublicationFilesRepository _publicationFilesRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ICurrentUserService _currentUserService;

        public PublicationService(
            IUnitOfWork unitOfWork,
            IPublicationRepository publicationRepository,
            IJournalArticleRepository journalArticleRepository,
            ITechnicalReportRepository technicalReportRepository,
            IBookChapterRepository bookChapterRepository,
            INationalConferenceRepository nationalConferenceRepository,
            IInternationalConferenceRepository internationalConferenceRepository,
            IBlobStorageService blobStorageService,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _publicationRepository = publicationRepository;
            _journalArticleRepository = journalArticleRepository;
            _technicalReportRepository = technicalReportRepository;
            _bookChapterRepository = bookChapterRepository;
            _nationalConferenceRepository = nationalConferenceRepository;
            _internationalConferenceRepository = internationalConferenceRepository;
            _blobStorageService = blobStorageService;
            _currentUserService = currentUserService;
        }

        // ─── Command-based (controller-facing) API ──────────────────────────────

        public async Task<Result<(List<PublicationDto> Publications, int Total)>> GetPublicationsAsync(GetPublicationsQuery getPublicationsQuery)
        {
            try
            {
                int page = getPublicationsQuery.Page < 1 ? 1 : getPublicationsQuery.Page;
                int limit = getPublicationsQuery.Limit < 1 ? 10 : getPublicationsQuery.Limit;

                PublicationType? parsedType = TryParseEnum<PublicationType>(getPublicationsQuery.Type);
                PublicationStatus? status = TryParseEnum<PublicationStatus>(getPublicationsQuery.Status);
                PublicationVisibility? visibility = _currentUserService.UserId != null ? null : PublicationVisibility.Public;

                var (items, total) = await _publicationRepository.GetFilteredAsync(
                    type: parsedType,
                    status: status,
                    visibility: visibility,
                    userId: getPublicationsQuery.UserId,
                    researchAxisId: getPublicationsQuery.AxeId,
                    year: getPublicationsQuery.Year,
                    search: getPublicationsQuery.Search,
                    page: page,
                    limit: limit);

                var list = items.Select(i => i.ToDto()).ToList();
                return Result<(List<PublicationDto> Publications, int Total)>.SuccessResult(data: (list, total));
            }
            catch (Exception ex)
            {
                return Result<(List<PublicationDto> publications, int total)>.FailureResult($"Error retrieving publications: {ex.Message}");
            }
        }

        public async Task<Result<PublicationDto>> GetPublicationByIdAsync(Guid publicationId)        
        {
            try
            {
                var p = await _publicationRepository.GetByIdAsync(publicationId);

                if (p is null || p.Status != PublicationStatus.Published)
                    return Result<PublicationDto>.FailureResult("Publication not found.");

                bool canAccess = _currentUserService?.UserId.Value != null || p.Visibility == PublicationVisibility.Public;
                if (!canAccess)
                    return Result<PublicationDto>.FailureResult("User is not authorized to see this publication.");

                return Result<PublicationDto>.SuccessResult(p.ToDto());
            }
            catch (Exception ex)
            {
                return Result<PublicationDto>.FailureResult($"Error retrieving publication: {ex.Message}");
            }
        }

        public async Task<Result<List<FileDownloadDto>>> GetPublicationPdfsAsync(Guid publicationId)
        {
            var publicationFiles = await _publicationFilesRepository.GetByPublicationIdAsync(publicationId);
            if (publicationFiles == null)
                return Result<List<FileDownloadDto>>.FailureResult("Attachments not found");

            try
            {
                List<FileDownloadDto> attachments = new List<FileDownloadDto>();
                foreach (var file in publicationFiles)
                {
                    var stream = await _blobStorageService.GetStreamAsync("media", file.BlobFileName);
                    attachments.Add(new FileDownloadDto
                    {
                        FileId = file.Id,
                        Stream = stream,
                        FileName = file.OriginalFileName,
                        ContentType = "application/pdf"
                    });
                }
                return Result<List<FileDownloadDto>>.SuccessResult(data: attachments);
            }
            catch (FileNotFoundException)
            {
                return Result<List<FileDownloadDto>>.FailureResult("Failed to get publications pdfs from blob storage");
            }
        }

        public async Task<Result<PublicationDto>> CreatePublicationAsync(CreatePublicationCommand command)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                if (_currentUserService.UserId == null)
                    return Result<PublicationDto>.FailureResult("User not authenticated.");

                var entity = command.Publication.ToEntity();
                entity.Id = Guid.NewGuid();
                entity.Status = PublicationStatus.Draft;

                await _publicationRepository.AddAsync(entity);

                await CreateSpecificPublicationDetails(command.Publication, entity.Id);

                bool added = await _unitOfWork.SaveChangesAsync() > 0;
                
                if (!added)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result<PublicationDto>.FailureResult("Failed to create publication.");
                }

                await _unitOfWork.CommitTransactionAsync();
                return Result<PublicationDto>.SuccessResult(entity.ToDto());
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<PublicationDto>.FailureResult($"Error creating publication: {ex.Message}");
            }
        }

        public async Task<Result<bool>> UpdatePublicationAsync(UpdatePublicationCommand command)
        {
            try
            {
                var existing = await _publicationRepository.GetByIdAsync(command.Publication.Id.Value);
                if (existing is null)
                    return Result<bool>.FailureResult("Publication not found.");

                bool isAdmin = IsAdminRole(_currentUserService.Role);
                bool isAuthor = _currentUserService.UserId.Value != Guid.Empty && existing.UserId == _currentUserService.UserId.Value;

                if (!isAdmin && !isAuthor)
                    return Result<bool>.FailureResult("You are not the owner of this publication.");

                if (!isAdmin &&
                    existing.Status is not (PublicationStatus.Draft or PublicationStatus.Rejected))
                    return Result<bool>.FailureResult("Submitted or published publications cannot be modified.");

                var entity = command.Publication.ToEntity();

                _publicationRepository.Update(entity);
                bool updated = await _unitOfWork.SaveChangesAsync() > 0;
                if (!updated)
                    return Result<bool>.FailureResult("Failed to update publication.");

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Error updating publication: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeletePublicationAsync(Guid publicationId)
        {
            try
            {
                var existing = await _publicationRepository.GetByIdAsync(publicationId);
                if (existing is null)
                    return Result<bool>.FailureResult("Publication not found.");

                bool isAdmin = IsAdminRole(_currentUserService.Role);
                bool isAuthor = _currentUserService.UserId.Value != null && existing.UserId == _currentUserService.UserId.Value;

                if (!isAdmin && !isAuthor)
                    return Result<bool>.FailureResult("User not authorized to delete publication.");

                if (!isAdmin && existing.Status != PublicationStatus.Draft)
                    return Result<bool>.FailureResult("Only publications in draft status can be deleted by non admin users.");

                _publicationRepository.Remove(existing);

                bool deleted = await _unitOfWork.SaveChangesAsync() > 0;
                if (!deleted)
                    return Result<bool>.FailureResult("Failed to delete publication.");

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Error deleting publication: {ex.Message}");
            }
        }

        public async Task<Result<List<FileDownloadDto>>> GetPublicationPdfs(Guid publicationId)
        {
            try
            {
                var publicationFiles = await _publicationFilesRepository.GetByPublicationIdAsync(publicationId);
                if (publicationFiles == null)
                    return Result<List<FileDownloadDto>>.FailureResult("Attachments not found");
                List<FileDownloadDto> attachments = new List<FileDownloadDto>();
                foreach (var file in publicationFiles)
                {
                    var stream = await _blobStorageService.GetStreamAsync("media", file.BlobFileName);
                    attachments.Add(new FileDownloadDto
                    {
                        FileId = file.Id,
                        Stream = stream,
                        FileName = file.OriginalFileName,
                        ContentType = "application/pdf"
                    });
                }
                return Result<List<FileDownloadDto>>.SuccessResult(data: attachments);
            }
            catch (FileNotFoundException)
            {
                return Result<List<FileDownloadDto>>.FailureResult("Failed to get publications pdfs from blob storage");
            }
            catch (Exception ex)
            {
                return Result<List<FileDownloadDto>>.FailureResult($"Error retrieving publication pdfs: {ex.Message}");
            }
        }

        public async Task<Result<bool>> AddPublicationPdfsAsync(AddPublicationPdfsCommand command)
        {
            try
            {
                var publication = await GetAndValidatePublicationAsync(command.PublicationId);

                var uploadResult = await UploadFilesAsync(command.FileDownloads, command.PublicationId);

                if (!uploadResult.Success)
                    return Result<bool>.FailureResult("Failed to upload files into blob storage");

                await SaveFilesWithTransactionAsync(uploadResult.Files);

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Error uploading pdf: {ex.Message}");
            }
        }

        public async Task<Result<bool>> RemovePublicationPdfAsync(RemovePublicationPdfCommand command)
        {
            try
            {
                var publication = await _publicationRepository.GetByIdAsync(command.PublicationId);
                if (publication is null)
                    return Result<bool>.FailureResult("Publication not found.");

                bool isAdmin = IsAdminRole(_currentUserService.Role);
                bool isAuthor = _currentUserService.UserId.Value != null &&
                                 publication.UserId == _currentUserService.UserId.Value;

                if (!isAdmin && !isAuthor)
                    return Result<bool>.FailureResult("You are not the owner of this publication.");

                var file = await _publicationFilesRepository.GetByIdAsync(command.FileId);

                if (file is null)
                    return Result<bool>.FailureResult("Pdf not found in publication attachments.");

                // 1. Delete from blob first
                var deleted = await _blobStorageService.DeleteBlobAsync(
                    "media",
                    file.BlobPath);

                if (!deleted)
                    return Result<bool>.FailureResult("Failed to delete pdf from blob storage.");

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // 2. Delete DB record
                    await _publicationFilesRepository.DeleteAsync(file);

                    var saved = await _unitOfWork.SaveChangesAsync() > 0;

                    if (!saved)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return Result<bool>.FailureResult("Failed to delete file from database.");
                    }

                    await _unitOfWork.CommitTransactionAsync();

                    return Result<bool>.SuccessResult(true);
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Error removing pdf: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SubmitPublicationAsync(Guid publicationId)
        {
            try
            {
                var existing = await _publicationRepository.GetByIdAsync(publicationId);
                if (existing is null)
                    return Result<bool>.FailureResult("Publication not found.");

                if (_currentUserService.UserId.Value == null || existing.UserId != _currentUserService.UserId.Value)
                    return Result<bool>.FailureResult("User not authorized to submit publication.");

                if (existing.Status != PublicationStatus.Draft && existing.Status != PublicationStatus.Rejected)
                    return Result<bool>.FailureResult("Only publications in draft or rejected status can be submitted.");

                existing.Status = PublicationStatus.Submitted;
                _publicationRepository.Update(existing);
                bool submitted = await _unitOfWork.SaveChangesAsync() > 0;

                if (!submitted)
                    return Result<bool>.FailureResult("Failed to submit publication.");

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Error submitting publication: {ex.Message}");
            }
        }

        public async Task<Result<bool>> ValidatePublicationAsync(Guid publicationId)
        {
            try
            {
                var existing = await _publicationRepository.GetByIdAsync(publicationId);
                if (existing is null)
                    return Result<bool>.FailureResult("Publication not found.");

                if (!IsAdminRole(_currentUserService.Role))
                    return Result<bool>.FailureResult("User not authorized to validate publication.");

                if (existing.Status != PublicationStatus.Submitted)
                    return Result<bool>.FailureResult("Only publications in submitted status can be validated.");

                existing.Status = PublicationStatus.Published;
                _publicationRepository.Update(existing);
                bool validated = await _unitOfWork.SaveChangesAsync() > 0;

                if (!validated)
                    return Result<bool>.FailureResult("Failed to validate publication.");

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Error validating publication: {ex.Message}");
            }
        }

        public async Task<Result<bool>> RejectPublicationAsync(Guid publicationId)
        {
            try
            {
                var existing = await _publicationRepository.GetByIdAsync(publicationId);
                if (existing is null)
                    return Result<bool>.FailureResult("Publication not found.");

                if (!IsAdminRole(_currentUserService.Role))
                    return Result<bool>.FailureResult("Access denied.");

                if (existing.Status != PublicationStatus.Submitted)
                    return Result<bool>.FailureResult("Only publications in submitted status can be rejected.");

                existing.Status = PublicationStatus.Rejected;
                _publicationRepository.Update(existing);
                bool rejected = await _unitOfWork.SaveChangesAsync() > 0;

                if (!rejected)
                    return Result<bool>.FailureResult("Failed to reject publication.");

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Error rejecting publication: {ex.Message}");
            }
        }

        private async Task CreateSpecificPublicationDetails(PublicationDto publication, Guid publicationId)
        {
            switch (publication.Type)
            {
                case PublicationType.ArticleJournal when publication.JournalArticle is not null:
                    var journalEntity = publication.JournalArticle.ToEntity();
                    journalEntity.Id = publicationId;
                    await _journalArticleRepository.AddAsync(journalEntity);
                    break;
                case PublicationType.TechnicalReport when publication.TechnicalReport is not null:
                    var reportEntity = publication.TechnicalReport.ToEntity();
                    reportEntity.Id = publicationId;
                    await _technicalReportRepository.AddAsync(reportEntity);
                    break;
                case PublicationType.BookChapter when publication.BookChapter is not null:
                    var bookChapterEntity = publication.BookChapter.ToEntity();
                    bookChapterEntity.Id = publicationId;
                    await _bookChapterRepository.AddAsync(bookChapterEntity);
                    break;
                case PublicationType.NationalConference when publication.NationalConference is not null:
                    var nationalConfEntity = publication.NationalConference.ToEntity();
                    nationalConfEntity.Id = publicationId;
                    await _nationalConferenceRepository.AddAsync(nationalConfEntity);
                    break;
                case PublicationType.InternationalConference when publication.InternationalConference is not null:
                    var internationalConfEntity = publication.InternationalConference.ToEntity();
                    internationalConfEntity.Id = publicationId;
                    await _internationalConferenceRepository.AddAsync(internationalConfEntity);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported publication type or missing details for type '{publication.Type}'.");
            }
        }

        private static bool IsAdminRole(string? role) =>
           string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
           string.Equals(role, "SuperAdmin", StringComparison.OrdinalIgnoreCase);

        private static TEnum? TryParseEnum<TEnum>(string? value)
            where TEnum : struct
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) ? parsed : null;
        }

        private async Task<PublicationEntity> GetAndValidatePublicationAsync(Guid publicationId)
        {
            var publication = await _publicationRepository.GetByIdAsync(publicationId);

            if (publication is null)
                throw new Exception("Publication not found.");

            bool isAdmin = IsAdminRole(_currentUserService.Role);
            bool isAuthor = _currentUserService.UserId.Value != null &&
                            publication.UserId == _currentUserService.UserId.Value;

            if (!isAdmin && !isAuthor)
                throw new Exception("You are not the owner of this publication.");

            return publication;
        }

        private async Task<(bool Success, List<PublicationFileEntity> Files)> UploadFilesAsync(
            IEnumerable<FileDownloadDto> files,
            Guid publicationId)
        {
            var uploadedFiles = new List<PublicationFileEntity>();

            foreach (var file in files)
            {
                var fileId = Guid.NewGuid();
                var extension = Path.GetExtension(file.FileName);

                var blobFileName = $"{fileId}{extension}";
                var blobPath = $"publications/{publicationId}/{fileId}";

                var success = await _blobStorageService.UploadStreamAsync(
                    file.Stream,
                    "media",
                    blobPath,
                    true);

                if (!success)
                {
                    await CleanupBlobsAsync(uploadedFiles);
                    return (false, []);
                }

                uploadedFiles.Add(new PublicationFileEntity
                {
                    Id = fileId,
                    PublicationId = publicationId,
                    OriginalFileName = file.FileName,
                    BlobFileName = blobFileName,
                    BlobPath = blobPath,
                    ContentType = file.ContentType,
                    UploadedAtUtc = DateTime.UtcNow
                });
            }

            return (true, uploadedFiles);
        }

        private async Task SaveFilesWithTransactionAsync(List<PublicationFileEntity> files)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _publicationFilesRepository.AddRangeAsync(files);

                var saved = await _unitOfWork.SaveChangesAsync() > 0;

                if (!saved)
                    throw new Exception("Failed to save file metadata.");

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                await CleanupBlobsAsync(files);
                throw;
            }
        }

        private async Task CleanupBlobsAsync(IEnumerable<PublicationFileEntity> files)
        {
            foreach (var file in files)
            {
                await _blobStorageService.DeleteBlobAsync("media", file.BlobPath);
            }
        }
    }
}
