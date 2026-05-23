using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Abstractions.Publication
{
    public interface IPublicationService
    {
        // ─── Single publication ────────────────────────────────────────────────

        /// <summary>Returns a publication by its unique identifier, including all type-specific details.</summary>
        Task<PublicationEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // ─── User-scoped queries ───────────────────────────────────────────────

        /// <summary>Returns all publications belonging to a specific user (any status).</summary>
        Task<IEnumerable<PublicationEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>Returns publications belonging to a specific user filtered by status.</summary>
        Task<IEnumerable<PublicationEntity>> GetByUserIdAndStatusAsync(Guid userId, PublicationStatus status, CancellationToken cancellationToken = default);

        // ─── Public / visibility queries ──────────────────────────────────────

        /// <summary>Returns all publications that are PUBLIQUE and PUBLIE (for the public-facing pages).</summary>
        Task<IEnumerable<PublicationEntity>> GetPublicPublicationsAsync(CancellationToken cancellationToken = default);

        /// <summary>Returns the N most recent public publications (for the home page widget).</summary>
        Task<IEnumerable<PublicationEntity>> GetRecentPublicPublicationsAsync(int limit = 3, CancellationToken cancellationToken = default);

        // ─── Research-axis queries ─────────────────────────────────────────────

        /// <summary>Returns all publications linked to a given research axis.</summary>
        Task<IEnumerable<PublicationEntity>> GetByResearchAxisIdAsync(Guid researchAxisId, CancellationToken cancellationToken = default);

        // ─── Type-specific queries ─────────────────────────────────────────────

        /// <summary>Returns all publications of a given type (e.g. all journal articles).</summary>
        Task<IEnumerable<PublicationEntity>> GetByTypeAsync(PublicationType type, CancellationToken cancellationToken = default);

        // ─── Filtered / paginated listing (admin dashboard) ───────────────────

        /// <summary>
        /// Returns a paged, filtered list of publications.
        /// All filter parameters are optional and can be combined.
        /// </summary>
        Task<(IEnumerable<PublicationEntity> Items, int TotalCount)> GetFilteredAsync(
            PublicationType? type = null,
            PublicationStatus? status = null,
            PublicationVisibility? visibility = null,
            Guid? userId = null,
            Guid? researchAxisId = null,
            int? year = null,
            string? search = null,   // searches Title, Abstract, Keywords, Authors
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);

        // ─── Type-specific detail accessors ───────────────────────────────────

        Task<JournalArticleEntity?> GetJournalArticleByPublicationIdAsync(Guid publicationId, CancellationToken cancellationToken = default);
        Task<TechnicalReportEntity?> GetTechnicalReportByPublicationIdAsync(Guid publicationId, CancellationToken cancellationToken = default);
        Task<BookChapterEntity?> GetBookChapterByPublicationIdAsync(Guid publicationId, CancellationToken cancellationToken = default);
        Task<NationalConferenceEntity?> GetNationalConferenceByPublicationIdAsync(Guid publicationId, CancellationToken cancellationToken = default);
        Task<InternationalConferenceEntity?> GetInternationalConferenceByPublicationIdAsync(Guid publicationId, CancellationToken cancellationToken = default);

        // ─── Write operations ─────────────────────────────────────────────────

        Task<PublicationEntity> CreateAsync(PublicationEntity publication, CancellationToken cancellationToken = default);
        Task<PublicationEntity> UpdateAsync(PublicationEntity publication, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        // ─── Workflow / status transitions ────────────────────────────────────

        /// <summary>Moves a BROUILLON to SOUMIS.</summary>
        Task<PublicationEntity> SubmitAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>Admin/SuperAdmin approves a submitted publication (SOUMIS → PUBLIE).</summary>
        Task<PublicationEntity> ApproveAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>Admin/SuperAdmin rejects a submitted publication (SOUMIS → REJETE).</summary>
        Task<PublicationEntity> RejectAsync(Guid id, string? reason = null, CancellationToken cancellationToken = default);

        // ─── PDF management ───────────────────────────────────────────────────

        /// <summary>Attaches a PDF URL to a publication (called after the file upload CDN step).</summary>
        Task<PublicationEntity> AddPdfAsync(Guid id, string pdfUrl, CancellationToken cancellationToken = default);

        /// <summary>Removes a specific PDF URL from a publication.</summary>
        Task<PublicationEntity> RemovePdfAsync(Guid id, string pdfUrl, CancellationToken cancellationToken = default);

        // ─── Statistics ───────────────────────────────────────────────────────

        Task<int> CountPublicPublicationsAsync(CancellationToken cancellationToken = default);
    }
}