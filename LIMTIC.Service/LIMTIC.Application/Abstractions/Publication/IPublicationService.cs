using LIMTIC.Application.DTOs.Publications;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Abstractions.Publication
{
    public interface IPublicationService
    {
        // ─── Single publication ────────────────────────────────────────────────

        /// <summary>Returns a fully-populated DTO including type-specific details.</summary>
        Task<PublicationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // ─── User-scoped queries ───────────────────────────────────────────────

        Task<IEnumerable<PublicationSummaryDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PublicationSummaryDto>> GetByUserIdAndStatusAsync(Guid userId, PublicationStatus status, CancellationToken cancellationToken = default);

        // ─── Public / visibility queries ──────────────────────────────────────

        Task<IEnumerable<PublicationSummaryDto>> GetPublicPublicationsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<PublicationSummaryDto>> GetRecentPublicPublicationsAsync(int limit = 3, CancellationToken cancellationToken = default);

        // ─── Research-axis queries ─────────────────────────────────────────────

        Task<IEnumerable<PublicationSummaryDto>> GetByResearchAxisIdAsync(Guid researchAxisId, CancellationToken cancellationToken = default);

        // ─── Type-specific queries ─────────────────────────────────────────────

        Task<IEnumerable<PublicationSummaryDto>> GetByTypeAsync(PublicationType type, CancellationToken cancellationToken = default);

        // ─── Filtered / paginated listing ─────────────────────────────────────

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

        // ─── Type-specific detail accessors ───────────────────────────────────

        Task<JournalArticleDto?> GetJournalArticleByPublicationIdAsync(Guid publicationId, CancellationToken cancellationToken = default);
        Task<TechnicalReportDto?> GetTechnicalReportByPublicationIdAsync(Guid publicationId, CancellationToken cancellationToken = default);
        Task<BookChapterDto?> GetBookChapterByPublicationIdAsync(Guid publicationId, CancellationToken cancellationToken = default);
        Task<NationalConferenceDto?> GetNationalConferenceByPublicationIdAsync(Guid publicationId, CancellationToken cancellationToken = default);
        Task<InternationalConferenceDto?> GetInternationalConferenceByPublicationIdAsync(Guid publicationId, CancellationToken cancellationToken = default);

        // ─── Write operations ──────────────────────────────────────────────────

        Task<PublicationDto> CreateAsync(PublicationEntity publication, CancellationToken cancellationToken = default);
        Task<PublicationDto> UpdateAsync(PublicationEntity publication, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        // ─── Visibility-only update ────────────────────────────────────────────

        /// <summary>Updates only the Visibility field without touching other data or status guards.</summary>
        Task<PublicationDto> UpdateVisibilityAsync(Guid id, PublicationVisibility visibility, CancellationToken cancellationToken = default);

        // ─── Workflow / status transitions ─────────────────────────────────────

        Task<PublicationDto> SubmitAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PublicationDto> ApproveAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PublicationDto> RejectAsync(Guid id, string? reason = null, CancellationToken cancellationToken = default);

        // ─── PDF management ────────────────────────────────────────────────────

        Task<PublicationDto> AddPdfAsync(Guid id, string pdfUrl, CancellationToken cancellationToken = default);
        Task<PublicationDto> RemovePdfAsync(Guid id, string pdfUrl, CancellationToken cancellationToken = default);

        // ─── Statistics ───────────────────────────────────────────────────────

        Task<int> CountPublicPublicationsAsync(CancellationToken cancellationToken = default);
    }
}