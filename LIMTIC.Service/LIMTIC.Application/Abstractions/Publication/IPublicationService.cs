using LIMTIC.Application.DTOs.Publications;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;
using LIMTIC.Application.Contracts.Commands.Publications;
using LIMTIC.Application.DTOs;

namespace LIMTIC.Application.Abstractions.Publication
{
    public interface IPublicationService
    {
        // ─── Command-based (controller-facing) API ──────────────────────────────

        Task<Result<PublicPublicationsListResultDto>> GetPublicPublicationsAsync(
            GetPublicPublicationsCommand command,
            CancellationToken cancellationToken = default);

        Task<Result<List<PublicPublicationCardDto>>> GetRecentPublicPublicationsAsync(
            GetRecentPublicPublicationsCommand command,
            CancellationToken cancellationToken = default);

        Task<Result<PublicPublicationDetailDto>> GetPublicPublicationByIdAsync(
            GetPublicPublicationByIdCommand command,
            CancellationToken cancellationToken = default);

        Task<Result<DashboardPublicationsListResultDto>> GetDashboardPublicationsAsync(
            GetDashboardPublicationsCommand command,
            CancellationToken cancellationToken = default);

        Task<Result<CreateDashboardPublicationResultDto>> CreateDashboardPublicationAsync(
            CreateDashboardPublicationCommand command,
            CancellationToken cancellationToken = default);

        Task<Result<DashboardPublicationDetailDto>> GetDashboardPublicationByIdAsync(
            GetDashboardPublicationByIdCommand command,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> UpdateDashboardPublicationAsync(
            UpdateDashboardPublicationCommand command,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteDashboardPublicationAsync(
            DeleteDashboardPublicationCommand command,
            CancellationToken cancellationToken = default);

        Task<Result<string>> AddDashboardPublicationPdfAsync(
            AddDashboardPublicationPdfCommand command,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> RemoveDashboardPublicationPdfAsync(
            RemoveDashboardPublicationPdfCommand command,
            CancellationToken cancellationToken = default);

        Task<Result<PublicationStatusUpdateResultDto>> SubmitDashboardPublicationAsync(
            SubmitDashboardPublicationCommand command,
            CancellationToken cancellationToken = default);

        Task<Result<PublicationStatusUpdateResultDto>> ValidateDashboardPublicationAsync(
            ValidateDashboardPublicationCommand command,
            CancellationToken cancellationToken = default);

        Task<Result<PublicationStatusUpdateResultDto>> RejectDashboardPublicationAsync(
            RejectDashboardPublicationCommand command,
            CancellationToken cancellationToken = default);

        /// <summary>Returns a fully-populated DTO including type-specific details.</summary>
        Task<PublicationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<PublicationSummaryDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PublicationSummaryDto>> GetByUserIdAndStatusAsync(Guid userId, PublicationStatus status, CancellationToken cancellationToken = default);

        Task<IEnumerable<PublicationSummaryDto>> GetPublicPublicationsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<PublicationSummaryDto>> GetRecentPublicPublicationsAsync(int limit = 3, CancellationToken cancellationToken = default);

        Task<IEnumerable<PublicationSummaryDto>> GetByResearchAxisIdAsync(Guid researchAxisId, CancellationToken cancellationToken = default);

        Task<IEnumerable<PublicationSummaryDto>> GetByTypeAsync(PublicationType type, CancellationToken cancellationToken = default);

        Task<(IEnumerable<PublicationSummaryDto> Items, int TotalCount)> GetFilteredAsync(
            PublicationType? type = null,
            PublicationStatus? status = null,
            PublicationVisibility? visibility = null,
            Guid? userId = null,
            Guid? researchAxisId = null,
            int? year = null,
            string? search = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);

        Task<JournalArticleDto?> GetJournalArticleByPublicationIdAsync(Guid publicationId, CancellationToken cancellationToken = default);
        Task<TechnicalReportDto?> GetTechnicalReportByPublicationIdAsync(Guid publicationId, CancellationToken cancellationToken = default);
        Task<BookChapterDto?> GetBookChapterByPublicationIdAsync(Guid publicationId, CancellationToken cancellationToken = default);
        Task<NationalConferenceDto?> GetNationalConferenceByPublicationIdAsync(Guid publicationId, CancellationToken cancellationToken = default);
        Task<InternationalConferenceDto?> GetInternationalConferenceByPublicationIdAsync(Guid publicationId, CancellationToken cancellationToken = default);

        Task<PublicationDto> CreateAsync(PublicationEntity publication, CancellationToken cancellationToken = default);
        Task<PublicationDto> UpdateAsync(PublicationEntity publication, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>Updates only the Visibility field without touching other data or status guards.</summary>
        Task<PublicationDto> UpdateVisibilityAsync(Guid id, PublicationVisibility visibility, CancellationToken cancellationToken = default);

        Task<PublicationDto> SubmitAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PublicationDto> ApproveAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PublicationDto> RejectAsync(Guid id, string? reason = null, CancellationToken cancellationToken = default);
        Task<PublicationDto> AddPdfAsync(Guid id, string pdfUrl, CancellationToken cancellationToken = default);
        Task<PublicationDto> RemovePdfAsync(Guid id, string pdfUrl, CancellationToken cancellationToken = default);

        Task<int> CountPublicPublicationsAsync(CancellationToken cancellationToken = default);
    }
}
