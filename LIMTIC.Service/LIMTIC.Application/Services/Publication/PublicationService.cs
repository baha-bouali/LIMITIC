using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.DTOs.Publications;
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

        public async Task<PublicationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _publicationRepository.GetByIdWithDetailsAsync(id, cancellationToken);
            return entity?.ToDto();
        }

        // ─── User-scoped queries ───────────────────────────────────────────────

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

        // ─── Public / visibility queries ───────────────────────────────────────

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

        // ─── Research-axis queries ─────────────────────────────────────────────

        public async Task<IEnumerable<PublicationSummaryDto>> GetByResearchAxisIdAsync(
            Guid researchAxisId, CancellationToken cancellationToken = default)
        {
            var entities = await _publicationRepository.GetByResearchAxisIdAsync(researchAxisId, cancellationToken);
            return entities.ToSummaryDtos();
        }

        // ─── Type-specific queries ─────────────────────────────────────────────

        public async Task<IEnumerable<PublicationSummaryDto>> GetByTypeAsync(
            PublicationType type, CancellationToken cancellationToken = default)
        {
            var entities = await _publicationRepository.GetByTypeAsync(type, cancellationToken);
            return entities.ToSummaryDtos();
        }

        // ─── Filtered / paginated listing ──────────────────────────────────────

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

        // ─── Type-specific detail accessors ───────────────────────────────────

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

        // ─── Write operations ──────────────────────────────────────────────────

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

        // ─── Visibility-only update ────────────────────────────────────────────

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

        // ─── Workflow / status transitions ─────────────────────────────────────

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
            // publication.RejectionReason = reason;  // uncomment once field exists

            _publicationRepository.Update(publication);
            await _publicationRepository.SaveChangesAsync(cancellationToken);

            return await GetFullDtoOrThrowAsync(id, cancellationToken);
        }

        // ─── PDF management ────────────────────────────────────────────────────

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

        // ─── Statistics ───────────────────────────────────────────────────────

        public async Task<int> CountPublicPublicationsAsync(CancellationToken cancellationToken = default)
        {
            return await _publicationRepository.CountByStatusAndVisibilityAsync(
                PublicationStatus.Published, PublicationVisibility.Public, cancellationToken);
        }

        // ─── Private helpers ───────────────────────────────────────────────────

        private async Task<PublicationDto> GetFullDtoOrThrowAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _publicationRepository.GetByIdWithDetailsAsync(id, cancellationToken)
                ?? throw new InvalidOperationException(
                    $"Publication {id} could not be retrieved after the operation.");
            return entity.ToDto();
        }
    }
}