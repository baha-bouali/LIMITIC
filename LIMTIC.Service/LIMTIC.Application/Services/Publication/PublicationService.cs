using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Services
{
    public class PublicationService : IPublicationService
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly IJournalArticleRepository _journalArticleRepository;
        private readonly ITechnicalReportRepository _technicalReportRepository;
        private readonly IBookChapterRepository _bookChapterRepository;
        private readonly INationalConferenceRepository _nationalConferenceRepository;
        private readonly IInternationalConferenceRepository _internationalConferenceRepository;

        public PublicationService(
            IPublicationRepository publicationRepository,
            IJournalArticleRepository journalArticleRepository,
            ITechnicalReportRepository technicalReportRepository,
            IBookChapterRepository bookChapterRepository,
            INationalConferenceRepository nationalConferenceRepository,
            IInternationalConferenceRepository internationalConferenceRepository)
        {
            _publicationRepository = publicationRepository;
            _journalArticleRepository = journalArticleRepository;
            _technicalReportRepository = technicalReportRepository;
            _bookChapterRepository = bookChapterRepository;
            _nationalConferenceRepository = nationalConferenceRepository;
            _internationalConferenceRepository = internationalConferenceRepository;
        }

        // ─── Single publication ────────────────────────────────────────────────

        public async Task<PublicationEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _publicationRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        }

        // ─── User-scoped queries ───────────────────────────────────────────────

        public async Task<IEnumerable<PublicationEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _publicationRepository.GetByUserIdAsync(userId, cancellationToken);
        }

        public async Task<IEnumerable<PublicationEntity>> GetByUserIdAndStatusAsync(
            Guid userId,
            PublicationStatus status,
            CancellationToken cancellationToken = default)
        {
            return await _publicationRepository.GetByUserIdAndStatusAsync(userId, status, cancellationToken);
        }

        // ─── Public / visibility queries ──────────────────────────────────────

        public async Task<IEnumerable<PublicationEntity>> GetPublicPublicationsAsync(CancellationToken cancellationToken = default)
        {
            return await _publicationRepository.GetByStatusAndVisibilityAsync(
                PublicationStatus.Published,
                PublicationVisibility.Public,
                cancellationToken);
        }

        public async Task<IEnumerable<PublicationEntity>> GetRecentPublicPublicationsAsync(
            int limit = 3,
            CancellationToken cancellationToken = default)
        {
            var publications = await _publicationRepository.GetByStatusAndVisibilityAsync(
                PublicationStatus.Published,
                PublicationVisibility.Public,
                cancellationToken);

            return publications
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.CreatedAtUtc)
                .Take(limit);
        }

        // ─── Research-axis queries ─────────────────────────────────────────────

        public async Task<IEnumerable<PublicationEntity>> GetByResearchAxisIdAsync(
            Guid researchAxisId,
            CancellationToken cancellationToken = default)
        {
            return await _publicationRepository.GetByResearchAxisIdAsync(researchAxisId, cancellationToken);
        }

        // ─── Type-specific queries ─────────────────────────────────────────────

        public async Task<IEnumerable<PublicationEntity>> GetByTypeAsync(
            PublicationType type,
            CancellationToken cancellationToken = default)
        {
            return await _publicationRepository.GetByTypeAsync(type, cancellationToken);
        }

        // ─── Filtered / paginated listing ─────────────────────────────────────

        public async Task<(IEnumerable<PublicationEntity> Items, int TotalCount)> GetFilteredAsync(
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
            return await _publicationRepository.GetFilteredAsync(
                type, status, visibility, userId, researchAxisId,
                year, search, page, pageSize, cancellationToken);
        }

        // ─── Type-specific detail accessors ───────────────────────────────────

        public async Task<JournalArticleEntity?> GetJournalArticleByPublicationIdAsync(
            Guid publicationId, CancellationToken cancellationToken = default)
        {
            return await _journalArticleRepository.GetByPublicationIdAsync(publicationId, cancellationToken);
        }

        public async Task<TechnicalReportEntity?> GetTechnicalReportByPublicationIdAsync(
            Guid publicationId, CancellationToken cancellationToken = default)
        {
            return await _technicalReportRepository.GetByPublicationIdAsync(publicationId, cancellationToken);
        }

        public async Task<BookChapterEntity?> GetBookChapterByPublicationIdAsync(
            Guid publicationId, CancellationToken cancellationToken = default)
        {
            return await _bookChapterRepository.GetByPublicationIdAsync(publicationId, cancellationToken);
        }

        public async Task<NationalConferenceEntity?> GetNationalConferenceByPublicationIdAsync(
            Guid publicationId, CancellationToken cancellationToken = default)
        {
            return await _nationalConferenceRepository.GetByPublicationIdAsync(publicationId, cancellationToken);
        }

        public async Task<InternationalConferenceEntity?> GetInternationalConferenceByPublicationIdAsync(
            Guid publicationId, CancellationToken cancellationToken = default)
        {
            return await _internationalConferenceRepository.GetByPublicationIdAsync(publicationId, cancellationToken);
        }

        // ─── Write operations ─────────────────────────────────────────────────

        public async Task<PublicationEntity> CreateAsync(
            PublicationEntity publication,
            CancellationToken cancellationToken = default)
        {
            publication.Status = PublicationStatus.Draft;

            await _publicationRepository.AddAsync(publication, cancellationToken);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return publication;
        }

        public async Task<PublicationEntity> UpdateAsync(
            PublicationEntity publication,
            CancellationToken cancellationToken = default)
        {
            var existing = await _publicationRepository.GetByIdAsync(publication.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Publication {publication.Id} not found.");

            // Guard: only drafts and rejected publications can be edited
            if (existing.Status is not (PublicationStatus.Draft or PublicationStatus.Rejected))
                throw new InvalidOperationException(
                    $"Publication {publication.Id} cannot be edited in status '{existing.Status}'.");

            _publicationRepository.Update(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return publication;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var publication = await _publicationRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Publication {id} not found.");

            _publicationRepository.Remove(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);
        }

        // ─── Workflow / status transitions ────────────────────────────────────

        public async Task<PublicationEntity> SubmitAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var publication = await _publicationRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Publication {id} not found.");

            if (publication.Status != PublicationStatus.Draft)
                throw new InvalidOperationException(
                    $"Only Draft publications can be submitted. Current status: '{publication.Status}'.");

            publication.Status = PublicationStatus.Submitted;

            _publicationRepository.Update(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return publication;
        }

        public async Task<PublicationEntity> ApproveAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var publication = await _publicationRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Publication {id} not found.");

            if (publication.Status != PublicationStatus.Submitted)
                throw new InvalidOperationException(
                    $"Only Submitted publications can be approved. Current status: '{publication.Status}'.");

            publication.Status = PublicationStatus.Published;

            _publicationRepository.Update(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return publication;
        }

        public async Task<PublicationEntity> RejectAsync(
            Guid id,
            string? reason = null,
            CancellationToken cancellationToken = default)
        {
            var publication = await _publicationRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Publication {id} not found.");

            if (publication.Status != PublicationStatus.Submitted)
                throw new InvalidOperationException(
                    $"Only Submitted publications can be rejected. Current status: '{publication.Status}'.");

            publication.Status = PublicationStatus.Rejected;
            // Persist the rejection reason if your entity / audit log supports it.
            // e.g. publication.RejectionReason = reason;

            _publicationRepository.Update(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return publication;
        }

        // ─── PDF management ───────────────────────────────────────────────────

        public async Task<PublicationEntity> AddPdfAsync(
            Guid id,
            string pdfUrl,
            CancellationToken cancellationToken = default)
        {
            var publication = await _publicationRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Publication {id} not found.");

            publication.AttachedPdfs = [.. (publication.AttachedPdfs ?? []), pdfUrl];

            _publicationRepository.Update(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return publication;
        }

        public async Task<PublicationEntity> RemovePdfAsync(
            Guid id,
            string pdfUrl,
            CancellationToken cancellationToken = default)
        {
            var publication = await _publicationRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Publication {id} not found.");

            publication.AttachedPdfs = (publication.AttachedPdfs ?? [])
                .Where(p => p != pdfUrl)
                .ToArray();

            _publicationRepository.Update(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return publication;
        }

        // ─── Statistics ───────────────────────────────────────────────────────

        public async Task<int> CountPublicPublicationsAsync(CancellationToken cancellationToken = default)
        {
            return await _publicationRepository.CountByStatusAndVisibilityAsync(
                PublicationStatus.Published,
                PublicationVisibility.Public,
                cancellationToken);
        }
    }
}