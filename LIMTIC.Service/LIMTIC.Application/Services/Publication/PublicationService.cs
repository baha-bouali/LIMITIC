using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.DTOs.Publications;
using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Contracts.Commands.Publications;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.Mappings;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Services.Publication
{
    public class PublicationService : IPublicationService
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly IJournalArticleRepository _journalArticleRepository;
        private readonly ITechnicalReportRepository _technicalReportRepository;
        private readonly IBookChapterRepository _bookChapterRepository;
        private readonly INationalConferenceRepository _nationalConferenceRepository;
        private readonly IInternationalConferenceRepository _internationalConferenceRepository;
        private readonly ICurrentUserService _currentUserService;

        public PublicationService(
            IPublicationRepository publicationRepository,
            IJournalArticleRepository journalArticleRepository,
            ITechnicalReportRepository technicalReportRepository,
            IBookChapterRepository bookChapterRepository,
            INationalConferenceRepository nationalConferenceRepository,
            IInternationalConferenceRepository internationalConferenceRepository,
            ICurrentUserService currentUserService)
        {
            _publicationRepository = publicationRepository;
            _journalArticleRepository = journalArticleRepository;
            _technicalReportRepository = technicalReportRepository;
            _bookChapterRepository = bookChapterRepository;
            _nationalConferenceRepository = nationalConferenceRepository;
            _internationalConferenceRepository = internationalConferenceRepository;
            _currentUserService = currentUserService;
        }

        // ─── Command-based (controller-facing) API ──────────────────────────────

        public async Task<Result<PublicPublicationsListResultDto>> GetPublicPublicationsAsync(
            GetPublicPublicationsCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                int page = command.Page < 1 ? 1 : command.Page;
                int limit = command.Limit < 1 ? 10 : command.Limit;

                PublicationType? parsedType = TryParseEnum<PublicationType>(command.Type);

                PublicationVisibility? visibility = command.IsAuthenticated
                    ? null
                    : PublicationVisibility.Public;

                var (items, total) = await GetFilteredAsync(
                    type: parsedType,
                    status: PublicationStatus.Published,
                    visibility: visibility,
                    userId: null,
                    researchAxisId: command.AxeId,
                    year: command.Year,
                    search: command.Search,
                    page: page,
                    pageSize: limit,
                    cancellationToken: cancellationToken);

                var list = items.ToList();
                int journals = list.Count(p => p.Type == PublicationType.ArticleJournal);
                int conferences = list.Count(p =>
                    p.Type is PublicationType.ConferenceInternational
                           or PublicationType.ConferenceNational);
                int totalPages = (int)Math.Ceiling(total / (double)limit);

                var data = list.Select(p => new PublicPublicationSummaryItemDto(
                    Id: p.Id,
                    Type: p.Type.ToString(),
                    PublicationType: p.Type.ToString(),
                    Year: p.Year,
                    Title: p.Title,
                    Authors: p.Authors,
                    Venue: p.Venue,
                    Doi: p.Doi,
                    Ranking: p.JournalRanking?.ToString(),
                    CoreRanking: p.CoreRanking?.ToString(),
                    Axe: new PublicationAxeDto(p.ResearchAxisId, p.ResearchAxisName)
                )).ToList();

                return Result<PublicPublicationsListResultDto>.SuccessResult(
                    new PublicPublicationsListResultDto(
                        Data: data,
                        Stats: new PublicationStatsDto(Total: total, Journals: journals, Conferences: conferences),
                        Pagination: new PaginationDto(Total: total, Page: page, Limit: limit, TotalPages: totalPages)));
            }
            catch (Exception ex)
            {
                return Result<PublicPublicationsListResultDto>.FailureResult($"Error retrieving publications: {ex.Message}");
            }
        }

        public async Task<Result<List<PublicPublicationCardDto>>> GetRecentPublicPublicationsAsync(
            GetRecentPublicPublicationsCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                int limit = command.Limit < 1 ? 3 : command.Limit;
                var publications = await GetRecentPublicPublicationsAsync(limit, cancellationToken);

                var cards = publications.Select(p => new PublicPublicationCardDto(
                    Id: p.Id,
                    Type: p.Type.ToString(),
                    PublicationType: p.Type.ToString(),
                    Year: p.Year,
                    Title: p.Title,
                    Authors: p.Authors,
                    Venue: p.Venue,
                    Doi: p.Doi,
                    Ranking: p.JournalRanking?.ToString(),
                    CoreRanking: p.CoreRanking?.ToString()
                )).ToList();

                return Result<List<PublicPublicationCardDto>>.SuccessResult(cards);
            }
            catch (Exception ex)
            {
                return Result<List<PublicPublicationCardDto>>.FailureResult($"Error retrieving publications: {ex.Message}");
            }
        }

        public async Task<Result<PublicPublicationDetailDto>> GetPublicPublicationByIdAsync(
            GetPublicPublicationByIdCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var p = await GetByIdAsync(command.Id, cancellationToken);
                if (p is null || p.Status != PublicationStatus.Published)
                    return Result<PublicPublicationDetailDto>.FailureResult("Publication not found.");

                bool canAccess = command.IsAuthenticated || p.Visibility == PublicationVisibility.Public;
                if (!canAccess)
                    return Result<PublicPublicationDetailDto>.FailureResult("Publication not found.");

                return Result<PublicPublicationDetailDto>.SuccessResult(MapDetail(p));
            }
            catch (Exception ex)
            {
                return Result<PublicPublicationDetailDto>.FailureResult($"Error retrieving publication: {ex.Message}");
            }
        }

        public async Task<Result<DashboardPublicationsListResultDto>> GetDashboardPublicationsAsync(
            GetDashboardPublicationsCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                int page = command.Page < 1 ? 1 : command.Page;
                int limit = command.Limit < 1 ? 10 : command.Limit;

                PublicationType? parsedType = TryParseEnum<PublicationType>(command.Type);
                PublicationStatus? parsedStatus = TryParseEnum<PublicationStatus>(command.Status);
                PublicationVisibility? parsedVisibility = TryParseEnum<PublicationVisibility>(command.Visibility);

                Guid? scopedUserId = string.Equals(command.Scope, "all", StringComparison.OrdinalIgnoreCase)
                    ? null
                    : (_currentUserService.UserId == Guid.Empty ? null : _currentUserService.UserId);

                var (items, total) = await GetFilteredAsync(
                    type: parsedType,
                    status: parsedStatus,
                    visibility: parsedVisibility,
                    userId: scopedUserId,
                    researchAxisId: command.AxeId,
                    year: command.Year,
                    search: command.Search,
                    page: page,
                    pageSize: limit,
                    cancellationToken: cancellationToken);

                int totalPages = (int)Math.Ceiling(total / (double)limit);

                var data = items.Select(p => new DashboardPublicationSummaryItemDto(
                    Id: p.Id,
                    Type: p.Type.ToString(),
                    Title: p.Title,
                    Year: p.Year,
                    Status: p.Status.ToString(),
                    Visibility: p.Visibility.ToString(),
                    Quartile: p.JournalRanking?.ToString(),
                    CoreRanking: p.CoreRanking?.ToString(),
                    Authors: p.Authors,
                    Venue: p.Venue,
                    Doi: p.Doi,
                    Axe: new PublicationAxeDto(p.ResearchAxisId, p.ResearchAxisName),
                    SubmittedBy: p.UserFullName,
                    RejectionReason: null
                )).ToList();

                return Result<DashboardPublicationsListResultDto>.SuccessResult(
                    new DashboardPublicationsListResultDto(
                        Data: data,
                        Pagination: new PaginationDto(total, page, limit, totalPages)));
            }
            catch (Exception ex)
            {
                return Result<DashboardPublicationsListResultDto>.FailureResult($"Error retrieving publications: {ex.Message}");
            }
        }

        public async Task<Result<CreateDashboardPublicationResultDto>> CreateDashboardPublicationAsync(
            CreateDashboardPublicationCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (_currentUserService.UserId == Guid.Empty)
                    return Result<CreateDashboardPublicationResultDto>.FailureResult("User not authenticated.");

                var createRequestResult = MapCreateCommand(command);
                if (!createRequestResult.Success)
                    return Result<CreateDashboardPublicationResultDto>.FailureResult(createRequestResult.Message!);

                var entity = createRequestResult.Data!.ToEntity();
                entity.UserId = _currentUserService.UserId;

                var created = await CreateAsync(entity, cancellationToken);
                await SubmitAsync(created.Id, cancellationToken);

                bool canPublishDirectly = IsAdminRole(_currentUserService.Role);
                if (canPublishDirectly)
                {
                    var approved = await ApproveAsync(created.Id, cancellationToken);
                    return Result<CreateDashboardPublicationResultDto>.SuccessResult(
                        new CreateDashboardPublicationResultDto(
                            Message: "Publication created and published successfully",
                            Id: approved.Id,
                            Status: approved.Status.ToString()));
                }

                return Result<CreateDashboardPublicationResultDto>.SuccessResult(
                    new CreateDashboardPublicationResultDto(
                        Message: "Publication submitted for validation",
                        Id: created.Id,
                        Status: PublicationStatus.Submitted.ToString()));
            }
            catch (Exception ex)
            {
                return Result<CreateDashboardPublicationResultDto>.FailureResult($"Error creating publication: {ex.Message}");
            }
        }

        public async Task<Result<DashboardPublicationDetailDto>> GetDashboardPublicationByIdAsync(
            GetDashboardPublicationByIdCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var p = await GetByIdAsync(command.Id, cancellationToken);
                if (p is null)
                    return Result<DashboardPublicationDetailDto>.FailureResult("Publication not found.");

                bool isAdmin = IsAdminRole(_currentUserService.Role);
                bool isAuthor = _currentUserService.UserId != Guid.Empty && p.UserId == _currentUserService.UserId;

                if (!isAdmin && !isAuthor)
                    return Result<DashboardPublicationDetailDto>.FailureResult("Access denied to this publication.");

                var detail = MapDetail(p);

                return Result<DashboardPublicationDetailDto>.SuccessResult(
                    new DashboardPublicationDetailDto(
                        Publication: detail,
                        SubmittedBy: p.UserFullName,
                        RejectionReason: null));
            }
            catch (Exception ex)
            {
                return Result<DashboardPublicationDetailDto>.FailureResult($"Error retrieving publication: {ex.Message}");
            }
        }

        public async Task<Result<bool>> UpdateDashboardPublicationAsync(
            UpdateDashboardPublicationCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var existing = await GetByIdAsync(command.Id, cancellationToken);
                if (existing is null)
                    return Result<bool>.FailureResult("Publication not found.");

                bool isAdmin = IsAdminRole(_currentUserService.Role);
                bool isAuthor = _currentUserService.UserId != Guid.Empty && existing.UserId == _currentUserService.UserId;

                if (!isAdmin && !isAuthor)
                    return Result<bool>.FailureResult("You are not the owner of this publication.");

                if (!isAdmin &&
                    existing.Status is not (PublicationStatus.Draft or PublicationStatus.Rejected))
                    return Result<bool>.FailureResult("Submitted or published publications cannot be modified.");

                var updateRequestResult = MapUpdateCommand(command);
                if (!updateRequestResult.Success)
                    return Result<bool>.FailureResult(updateRequestResult.Message!);

                var entity = updateRequestResult.Data!.ToEntity(command.Id);
                entity.UserId = existing.UserId;

                await UpdateAsync(entity, cancellationToken);
                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Error updating publication: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteDashboardPublicationAsync(
            DeleteDashboardPublicationCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var existing = await GetByIdAsync(command.Id, cancellationToken);
                if (existing is null)
                    return Result<bool>.FailureResult("Publication not found.");

                bool isAdmin = IsAdminRole(_currentUserService.Role);
                bool isAuthor = _currentUserService.UserId != Guid.Empty && existing.UserId == _currentUserService.UserId;

                if (!isAdmin && !isAuthor)
                    return Result<bool>.FailureResult("You are not the owner of this publication.");

                if (!isAdmin && existing.Status != PublicationStatus.Draft)
                    return Result<bool>.FailureResult("Only drafts can be deleted.");

                await DeleteAsync(command.Id, cancellationToken);
                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Error deleting publication: {ex.Message}");
            }
        }

        public async Task<Result<string>> AddDashboardPublicationPdfAsync(
            AddDashboardPublicationPdfCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var existing = await GetByIdAsync(command.Id, cancellationToken);
                if (existing is null)
                    return Result<string>.FailureResult("Publication not found.");

                bool isAdmin = IsAdminRole(_currentUserService.Role);
                bool isAuthor = _currentUserService.UserId != Guid.Empty && existing.UserId == _currentUserService.UserId;

                if (!isAdmin && !isAuthor)
                    return Result<string>.FailureResult("You are not the owner of this publication.");

                await AddPdfAsync(command.Id, command.PdfUrl, cancellationToken);
                return Result<string>.SuccessResult(command.PdfUrl);
            }
            catch (Exception ex)
            {
                return Result<string>.FailureResult($"Error uploading pdf: {ex.Message}");
            }
        }

        public async Task<Result<bool>> RemoveDashboardPublicationPdfAsync(
            RemoveDashboardPublicationPdfCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var existing = await GetByIdAsync(command.Id, cancellationToken);
                if (existing is null)
                    return Result<bool>.FailureResult("Publication not found.");

                bool isAdmin = IsAdminRole(_currentUserService.Role);
                bool isAuthor = _currentUserService.UserId != Guid.Empty && existing.UserId == _currentUserService.UserId;

                if (!isAdmin && !isAuthor)
                    return Result<bool>.FailureResult("You are not the owner of this publication.");

                await RemovePdfAsync(command.Id, command.PdfUrl, cancellationToken);
                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult($"Error removing pdf: {ex.Message}");
            }
        }

        public async Task<Result<PublicationStatusUpdateResultDto>> SubmitDashboardPublicationAsync(
            SubmitDashboardPublicationCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var existing = await GetByIdAsync(command.Id, cancellationToken);
                if (existing is null)
                    return Result<PublicationStatusUpdateResultDto>.FailureResult("Publication not found.");

                if (_currentUserService.UserId == Guid.Empty || existing.UserId != _currentUserService.UserId)
                    return Result<PublicationStatusUpdateResultDto>.FailureResult("You are not the owner of this publication.");

                var updated = await SubmitAsync(command.Id, cancellationToken);
                return Result<PublicationStatusUpdateResultDto>.SuccessResult(
                    new PublicationStatusUpdateResultDto(
                        Message: "Publication submitted for validation",
                        Id: updated.Id,
                        Status: updated.Status.ToString(),
                        Extra: null));
            }
            catch (Exception ex)
            {
                return Result<PublicationStatusUpdateResultDto>.FailureResult($"Error submitting publication: {ex.Message}");
            }
        }

        public async Task<Result<PublicationStatusUpdateResultDto>> ValidateDashboardPublicationAsync(
            ValidateDashboardPublicationCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!IsAdminRole(_currentUserService.Role))
                    return Result<PublicationStatusUpdateResultDto>.FailureResult("Access denied.");

                var publication = await ApproveAsync(command.Id, cancellationToken);
                return Result<PublicationStatusUpdateResultDto>.SuccessResult(
                    new PublicationStatusUpdateResultDto(
                        Message: "Publication validated and published successfully",
                        Id: publication.Id,
                        Status: publication.Status.ToString(),
                        Extra: publication.UserFullName));
            }
            catch (Exception ex)
            {
                return Result<PublicationStatusUpdateResultDto>.FailureResult($"Error validating publication: {ex.Message}");
            }
        }

        public async Task<Result<PublicationStatusUpdateResultDto>> RejectDashboardPublicationAsync(
            RejectDashboardPublicationCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!IsAdminRole(_currentUserService.Role))
                    return Result<PublicationStatusUpdateResultDto>.FailureResult("Access denied.");

                var publication = await RejectAsync(command.Id, command.Reason, cancellationToken);
                return Result<PublicationStatusUpdateResultDto>.SuccessResult(
                    new PublicationStatusUpdateResultDto(
                        Message: "Publication rejected",
                        Id: publication.Id,
                        Status: publication.Status.ToString(),
                        Extra: command.Reason));
            }
            catch (Exception ex)
            {
                return Result<PublicationStatusUpdateResultDto>.FailureResult($"Error rejecting publication: {ex.Message}");
            }
        }

        public async Task<PublicationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _publicationRepository.GetByIdWithDetailsAsync(id, cancellationToken);
            return entity?.ToDto();
        }

        public async Task<IEnumerable<PublicationSummaryDto>> GetByUserIdAsync(
            Guid userId, CancellationToken cancellationToken = default)
        {
            var entities = await _publicationRepository.GetByUserIdAsync(userId, cancellationToken);
            return entities.ToSummaryDtos();
        }

        public async Task<IEnumerable<PublicationSummaryDto>> GetByUserIdAndStatusAsync(
            Guid userId, PublicationStatus status, CancellationToken cancellationToken = default)
        {
            var entities = await _publicationRepository.GetByUserIdAndStatusAsync(userId, status, cancellationToken);
            return entities.ToSummaryDtos();
        }

        public async Task<IEnumerable<PublicationSummaryDto>> GetPublicPublicationsAsync(
            CancellationToken cancellationToken = default)
        {
            var entities = await _publicationRepository.GetByStatusAndVisibilityAsync(
                PublicationStatus.Published, PublicationVisibility.Public, cancellationToken);
            return entities.ToSummaryDtos();
        }

        public async Task<IEnumerable<PublicationSummaryDto>> GetRecentPublicPublicationsAsync(
            int limit = 3, CancellationToken cancellationToken = default)
        {
            var entities = await _publicationRepository.GetByStatusAndVisibilityAsync(
                PublicationStatus.Published, PublicationVisibility.Public, cancellationToken);

            return entities
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.CreatedAtUtc)
                .Take(limit)
                .ToSummaryDtos();
        }

        public async Task<IEnumerable<PublicationSummaryDto>> GetByResearchAxisIdAsync(
            Guid researchAxisId, CancellationToken cancellationToken = default)
        {
            var entities = await _publicationRepository.GetByResearchAxisIdAsync(researchAxisId, cancellationToken);
            return entities.ToSummaryDtos();
        }

        public async Task<IEnumerable<PublicationSummaryDto>> GetByTypeAsync(
            PublicationType type, CancellationToken cancellationToken = default)
        {
            var entities = await _publicationRepository.GetByTypeAsync(type, cancellationToken);
            return entities.ToSummaryDtos();
        }

        public async Task<(IEnumerable<PublicationSummaryDto> Items, int TotalCount)> GetFilteredAsync(
            PublicationType? type = null,
            PublicationStatus? status = null,
            PublicationVisibility? visibility = null,
            Guid? userId = null,
            Guid? researchAxisId = null,
            int? year = null,
            string? search = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var (items, totalCount) = await _publicationRepository.GetFilteredAsync(
                type, status, visibility, userId, researchAxisId,
                year, search, page, pageSize, cancellationToken);

            return (items.ToSummaryDtos(), totalCount);
        }

        public async Task<JournalArticleDto?> GetJournalArticleByPublicationIdAsync(
            Guid publicationId, CancellationToken cancellationToken = default)
        {
            var entity = await _journalArticleRepository.GetByPublicationIdAsync(publicationId, cancellationToken);
            return entity?.ToDto();
        }

        public async Task<TechnicalReportDto?> GetTechnicalReportByPublicationIdAsync(
            Guid publicationId, CancellationToken cancellationToken = default)
        {
            var entity = await _technicalReportRepository.GetByPublicationIdAsync(publicationId, cancellationToken);
            return entity?.ToDto();
        }

        public async Task<BookChapterDto?> GetBookChapterByPublicationIdAsync(
            Guid publicationId, CancellationToken cancellationToken = default)
        {
            var entity = await _bookChapterRepository.GetByPublicationIdAsync(publicationId, cancellationToken);
            return entity?.ToDto();
        }

        public async Task<NationalConferenceDto?> GetNationalConferenceByPublicationIdAsync(
            Guid publicationId, CancellationToken cancellationToken = default)
        {
            var entity = await _nationalConferenceRepository.GetByPublicationIdAsync(publicationId, cancellationToken);
            return entity?.ToDto();
        }

        public async Task<InternationalConferenceDto?> GetInternationalConferenceByPublicationIdAsync(
            Guid publicationId, CancellationToken cancellationToken = default)
        {
            var entity = await _internationalConferenceRepository.GetByPublicationIdAsync(publicationId, cancellationToken);
            return entity?.ToDto();
        }

        public async Task<PublicationDto> CreateAsync(
            PublicationEntity publication, CancellationToken cancellationToken = default)
        {
            publication.Status = PublicationStatus.Draft;

            await _publicationRepository.AddAsync(publication, cancellationToken);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return await GetFullDtoOrThrowAsync(publication.Id, cancellationToken);
        }

        public async Task<PublicationDto> UpdateAsync(
            PublicationEntity publication, CancellationToken cancellationToken = default)
        {
            var existing = await _publicationRepository.GetByIdAsync(publication.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Publication {publication.Id} not found.");

            if (existing.Status is not (PublicationStatus.Draft or PublicationStatus.Rejected))
                throw new InvalidOperationException(
                    $"Publication {publication.Id} cannot be edited in status '{existing.Status}'.");

            _publicationRepository.Update(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return await GetFullDtoOrThrowAsync(publication.Id, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var publication = await _publicationRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Publication {id} not found.");

            _publicationRepository.Remove(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<PublicationDto> UpdateVisibilityAsync(
            Guid id, PublicationVisibility visibility, CancellationToken cancellationToken = default)
        {
            var publication = await _publicationRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Publication {id} not found.");

            publication.Visibility = visibility;

            _publicationRepository.Update(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return await GetFullDtoOrThrowAsync(id, cancellationToken);
        }

        public async Task<PublicationDto> SubmitAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var publication = await _publicationRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Publication {id} not found.");

            if (publication.Status != PublicationStatus.Draft)
                throw new InvalidOperationException(
                    $"Only Draft publications can be submitted. Current status: '{publication.Status}'.");

            publication.Status = PublicationStatus.Submitted;
            _publicationRepository.Update(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return await GetFullDtoOrThrowAsync(id, cancellationToken);
        }

        public async Task<PublicationDto> ApproveAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var publication = await _publicationRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Publication {id} not found.");

            if (publication.Status != PublicationStatus.Submitted)
                throw new InvalidOperationException(
                    $"Only Submitted publications can be approved. Current status: '{publication.Status}'.");

            publication.Status = PublicationStatus.Published;
            _publicationRepository.Update(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return await GetFullDtoOrThrowAsync(id, cancellationToken);
        }

        public async Task<PublicationDto> RejectAsync(
            Guid id, string? reason = null, CancellationToken cancellationToken = default)
        {
            var publication = await _publicationRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Publication {id} not found.");

            if (publication.Status != PublicationStatus.Submitted)
                throw new InvalidOperationException(
                    $"Only Submitted publications can be rejected. Current status: '{publication.Status}'.");

            publication.Status = PublicationStatus.Rejected;
            _publicationRepository.Update(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return await GetFullDtoOrThrowAsync(id, cancellationToken);
        }

        public async Task<PublicationDto> AddPdfAsync(
            Guid id, string pdfUrl, CancellationToken cancellationToken = default)
        {
            var publication = await _publicationRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Publication {id} not found.");

            publication.AttachedPdfs = [.. (publication.AttachedPdfs ?? []), pdfUrl];

            _publicationRepository.Update(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return await GetFullDtoOrThrowAsync(id, cancellationToken);
        }

        public async Task<PublicationDto> RemovePdfAsync(
            Guid id, string pdfUrl, CancellationToken cancellationToken = default)
        {
            var publication = await _publicationRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Publication {id} not found.");

            publication.AttachedPdfs = (publication.AttachedPdfs ?? [])
                .Where(p => p != pdfUrl)
                .ToArray();

            _publicationRepository.Update(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return await GetFullDtoOrThrowAsync(id, cancellationToken);
        }

        public async Task<int> CountPublicPublicationsAsync(CancellationToken cancellationToken = default)
        {
            return await _publicationRepository.CountByStatusAndVisibilityAsync(
                PublicationStatus.Published, PublicationVisibility.Public, cancellationToken);
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

        private static PublicPublicationDetailDto MapDetail(PublicationDto p)
        {
            return new PublicPublicationDetailDto(
                Id: p.Id,
                Type: p.Type.ToString(),
                PublicationType: p.Type.ToString(),
                Status: p.Status.ToString(),
                Visibility: p.Visibility.ToString(),
                Title: p.Title,
                Year: p.Year,
                Authors: p.Authors,
                Abstract_: p.Abstract,
                Keywords: p.Keywords,
                Doi: p.Doi,
                Venue: p.Venue,
                PdfUrl: p.AttachedPdfs.FirstOrDefault(),
                Axe: new PublicationAxeDto(p.ResearchAxisId, p.ResearchAxisName),
                JournalName: p.JournalArticle?.JournalName,
                Volume: p.JournalArticle?.Volume,
                Number: p.JournalArticle?.Number,
                Pages: p.JournalArticle?.Pages
                       ?? p.BookChapter?.Pages
                       ?? p.NationalConference?.Pages
                       ?? p.InternationalConference?.Pages,
                Ranking: p.JournalArticle?.RankingLabel,
                CoreRanking: p.InternationalConference?.RankingLabel,
                Location: p.InternationalConference?.Location
                          ?? p.NationalConference?.Location,
                BookTitle: p.BookChapter?.BookTitle,
                Publisher: p.BookChapter?.Publisher,
                Isbn: p.BookChapter?.Isbn,
                ReportNumber: p.TechnicalReport?.ReportNumber,
                Institution: p.TechnicalReport?.Institution);
        }

        private static Result<CreatePublicationRequest> MapCreateCommand(CreateDashboardPublicationCommand command)
        {
            var type = TryParseEnum<PublicationType>(command.Type);
            if (type is null)
                return Result<CreatePublicationRequest>.FailureResult("Invalid publication type");

            var visibility = TryParseEnum<PublicationVisibility>(command.Visibility);
            if (visibility is null)
                return Result<CreatePublicationRequest>.FailureResult("Invalid publication visibility");

            CreateJournalArticleRequest? journalArticle = null;
            if (command.JournalArticle is not null)
            {
                var ranking = TryParseEnum<JournalRanking>(command.JournalArticle.Ranking);
                if (ranking is null)
                    return Result<CreatePublicationRequest>.FailureResult("Invalid journal ranking");

                journalArticle = new CreateJournalArticleRequest(
                    command.JournalArticle.JournalName,
                    command.JournalArticle.Volume,
                    command.JournalArticle.Number,
                    command.JournalArticle.Pages,
                    ranking.Value);
            }

            CreateInternationalConferenceRequest? internationalConference = null;
            if (command.InternationalConference is not null)
            {
                var ranking = TryParseEnum<CoreRanking>(command.InternationalConference.Ranking);
                if (ranking is null)
                    return Result<CreatePublicationRequest>.FailureResult("Invalid core ranking");

                internationalConference = new CreateInternationalConferenceRequest(
                    command.InternationalConference.ConferenceName,
                    command.InternationalConference.Location,
                    command.InternationalConference.Pages,
                    ranking.Value);
            }

            var request = new CreatePublicationRequest(
                ResearchAxisId: command.ResearchAxisId,
                Title: command.Title,
                Abstract: command.Abstract,
                Keywords: command.Keywords,
                Doi: command.Doi,
                Venue: command.Venue,
                Type: type.Value,
                Visibility: visibility.Value,
                Year: command.Year,
                Authors: command.Authors,
                JournalArticle: journalArticle,
                TechnicalReport: command.TechnicalReport is null
                    ? null
                    : new CreateTechnicalReportRequest(command.TechnicalReport.ReportNumber, command.TechnicalReport.Institution),
                BookChapter: command.BookChapter is null
                    ? null
                    : new CreateBookChapterRequest(command.BookChapter.BookTitle, command.BookChapter.Publisher, command.BookChapter.Isbn, command.BookChapter.Pages),
                NationalConference: command.NationalConference is null
                    ? null
                    : new CreateNationalConferenceRequest(command.NationalConference.ConferenceName, command.NationalConference.Location, command.NationalConference.Pages),
                InternationalConference: internationalConference);

            return Result<CreatePublicationRequest>.SuccessResult(request);
        }

        private static Result<UpdatePublicationRequest> MapUpdateCommand(UpdateDashboardPublicationCommand command)
        {
            var type = TryParseEnum<PublicationType>(command.Type);
            if (type is null)
                return Result<UpdatePublicationRequest>.FailureResult("Invalid publication type");

            var visibility = TryParseEnum<PublicationVisibility>(command.Visibility);
            if (visibility is null)
                return Result<UpdatePublicationRequest>.FailureResult("Invalid publication visibility");

            UpdateJournalArticleRequest? journalArticle = null;
            if (command.JournalArticle is not null)
            {
                var ranking = TryParseEnum<JournalRanking>(command.JournalArticle.Ranking);
                if (ranking is null)
                    return Result<UpdatePublicationRequest>.FailureResult("Invalid journal ranking");

                journalArticle = new UpdateJournalArticleRequest(
                    command.JournalArticle.JournalName,
                    command.JournalArticle.Volume,
                    command.JournalArticle.Number,
                    command.JournalArticle.Pages,
                    ranking.Value);
            }

            UpdateInternationalConferenceRequest? internationalConference = null;
            if (command.InternationalConference is not null)
            {
                var ranking = TryParseEnum<CoreRanking>(command.InternationalConference.Ranking);
                if (ranking is null)
                    return Result<UpdatePublicationRequest>.FailureResult("Invalid core ranking");

                internationalConference = new UpdateInternationalConferenceRequest(
                    command.InternationalConference.ConferenceName,
                    command.InternationalConference.Location,
                    command.InternationalConference.Pages,
                    ranking.Value);
            }

            var request = new UpdatePublicationRequest(
                ResearchAxisId: command.ResearchAxisId,
                Title: command.Title,
                Abstract: command.Abstract,
                Keywords: command.Keywords,
                Doi: command.Doi,
                Venue: command.Venue,
                Type: type.Value,
                Visibility: visibility.Value,
                Year: command.Year,
                Authors: command.Authors,
                JournalArticle: journalArticle,
                TechnicalReport: command.TechnicalReport is null
                    ? null
                    : new UpdateTechnicalReportRequest(command.TechnicalReport.ReportNumber, command.TechnicalReport.Institution),
                BookChapter: command.BookChapter is null
                    ? null
                    : new UpdateBookChapterRequest(command.BookChapter.BookTitle, command.BookChapter.Publisher, command.BookChapter.Isbn, command.BookChapter.Pages),
                NationalConference: command.NationalConference is null
                    ? null
                    : new UpdateNationalConferenceRequest(command.NationalConference.ConferenceName, command.NationalConference.Location, command.NationalConference.Pages),
                InternationalConference: internationalConference);

            return Result<UpdatePublicationRequest>.SuccessResult(request);
        }

        private async Task<PublicationDto> GetFullDtoOrThrowAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _publicationRepository.GetByIdWithDetailsAsync(id, cancellationToken)
                ?? throw new InvalidOperationException(
                    $"Publication {id} could not be retrieved after the operation.");
            return entity.ToDto();
        }
    }
}
