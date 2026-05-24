using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.Interfaces.Services;
using LIMTIC.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.API.Controllers.Dashboard
{
    // ══════════════════════════════════════════════════════════════════════════
    // PhD Student — own publications
    // Route: /api/v1/dashboard/phd-student/publications
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Doctorant: CRUD on own publications + submit action.
    /// Lab publications (all published, including private) → PhdStudentLabPublicationsController.
    /// </summary>
    [ApiController]
    [Route("api/v1/dashboard/phd-student/publications")]
    [Authorize(Roles = "PhdStudent")]
    public class PhdStudentPublicationsController : ControllerBase
    {
        private readonly IPublicationService _publicationService;
        private readonly ICurrentUserService _currentUserService;

        public PhdStudentPublicationsController(
            IPublicationService publicationService,
            ICurrentUserService currentUserService)
        {
            _publicationService = publicationService;
            _currentUserService = currentUserService;
        }

        // ── GET /dashboard/phd-student/publications ────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetMyPublications(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] string? type   = null,
            [FromQuery] int     page   = 1,
            [FromQuery] int     limit  = 10,
            CancellationToken   ct     = default)
        {
            PublicationType? parsedType = null;
            if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<PublicationType>(type, true, out var t))
                parsedType = t;

            PublicationStatus? parsedStatus = null;
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<PublicationStatus>(status, true, out var s))
                parsedStatus = s;

            var (items, total) = await _publicationService.GetFilteredAsync(
                type:              parsedType,
                status:            parsedStatus,
                visibility:        null,
                userId:            _currentUserService.UserId,
                researchAxisId:    null,
                year:              null,
                search:            search,
                page:              page,
                pageSize:          limit,
                cancellationToken: ct);

            int totalPages = (int)Math.Ceiling(total / (double)limit);

            return Ok(new
            {
                data = items.Select(p => new
                {
                    id              = p.Id,
                    type            = p.Type.ToString(),
                    title           = p.Title,
                    status          = p.Status.ToString(),
                    quartile        = p.JournalArticle?.Ranking.ToString(),
                    coreRanking     = p.InternationalConference?.Ranking.ToString(),
                    year            = p.Year,
                    authors         = string.Join(", ", p.Authors),
                    venue           = p.Venue,
                    rejectionReason = (string?)null
                }),
                pagination = new { total, page, limit, totalPages }
            });
        }

        // ── POST /dashboard/phd-student/publications ───────────────────────────
        [HttpPost]
        public async Task<IActionResult> CreatePublication(
            [FromBody] LIMTIC.Domain.Entities.Publications.PublicationEntity publication,
            CancellationToken ct = default)
        {
            publication.UserId = _currentUserService.UserId;

            var created = await _publicationService.CreateAsync(publication, ct);
            await _publicationService.SubmitAsync(created.Id, ct);

            return StatusCode(201, new
            {
                message     = "Publication soumise pour validation par l'administrateur",
                publication = new { id = created.Id, status = PublicationStatus.Submitted.ToString() }
            });
        }

        // ── PUT /dashboard/phd-student/publications/{id} ──────────────────────
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePublication(
            Guid id,
            [FromBody] LIMTIC.Domain.Entities.Publications.PublicationEntity publication,
            CancellationToken ct = default)
        {
            var existing = await _publicationService.GetByIdAsync(id, ct);
            if (existing is null)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            if (existing.UserId != _currentUserService.UserId)
                return StatusCode(403, new { error = "NOT_OWNER", message = "Vous n'êtes pas propriétaire de cette publication." });

            if (existing.Status is not (PublicationStatus.Draft or PublicationStatus.Rejected))
                return StatusCode(403, new
                {
                    error   = "PUBLICATION_NOT_EDITABLE",
                    message = "Les publications soumises ou publiées ne peuvent pas être modifiées."
                });

            publication.Id = id;
            await _publicationService.UpdateAsync(publication, ct);
            return Ok(new { message = "Publication mise à jour" });
        }

        // ── DELETE /dashboard/phd-student/publications/{id} ───────────────────
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePublication(Guid id, CancellationToken ct = default)
        {
            var existing = await _publicationService.GetByIdAsync(id, ct);
            if (existing is null)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            if (existing.UserId != _currentUserService.UserId)
                return StatusCode(403, new { error = "NOT_OWNER", message = "Vous n'êtes pas propriétaire de cette publication." });

            if (existing.Status != PublicationStatus.Draft)
                return StatusCode(403, new
                {
                    error   = "PUBLICATION_NOT_EDITABLE",
                    message = "Seuls les brouillons peuvent être supprimés."
                });

            await _publicationService.DeleteAsync(id, ct);
            return Ok(new { message = "Publication supprimée" });
        }

        // ── POST /dashboard/phd-student/publications/{id}/submit ──────────────
        [HttpPost("{id:guid}/submit")]
        public async Task<IActionResult> SubmitPublication(Guid id, CancellationToken ct = default)
        {
            var existing = await _publicationService.GetByIdAsync(id, ct);
            if (existing is null)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            if (existing.UserId != _currentUserService.UserId)
                return StatusCode(403, new { error = "NOT_OWNER", message = "Vous n'êtes pas propriétaire de cette publication." });

            var updated = await _publicationService.SubmitAsync(id, ct);
            return Ok(new { message = "Publication soumise pour validation", status = updated.Status.ToString() });
        }

        // ── POST /dashboard/phd-student/publications/{id}/pdf ─────────────────
        [HttpPost("{id:guid}/pdf")]
        public async Task<IActionResult> AddPdf(
            Guid id,
            [FromBody] PdfRequest request,
            CancellationToken ct = default)
        {
            var existing = await _publicationService.GetByIdAsync(id, ct);
            if (existing is null)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            if (existing.UserId != _currentUserService.UserId)
                return StatusCode(403, new { error = "NOT_OWNER", message = "Vous n'êtes pas propriétaire de cette publication." });

            await _publicationService.AddPdfAsync(id, request.PdfUrl, ct);
            return Ok(new { message = "PDF uploadé avec succès", pdfUrl = request.PdfUrl });
        }

        // ── DELETE /dashboard/phd-student/publications/{id}/pdf ───────────────
        [HttpDelete("{id:guid}/pdf")]
        public async Task<IActionResult> RemovePdf(
            Guid id,
            [FromBody] PdfRequest request,
            CancellationToken ct = default)
        {
            var existing = await _publicationService.GetByIdAsync(id, ct);
            if (existing is null)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            if (existing.UserId != _currentUserService.UserId)
                return StatusCode(403, new { error = "NOT_OWNER", message = "Vous n'êtes pas propriétaire de cette publication." });

            await _publicationService.RemovePdfAsync(id, request.PdfUrl, ct);
            return Ok(new { message = "PDF supprimé" });
        }

        public record PdfRequest(string PdfUrl);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // PhD Student — all lab publications (read-only, includes private ones)
    // Route: /api/v1/dashboard/phd-student/all-publications
    // ══════════════════════════════════════════════════════════════════════════

    [ApiController]
    [Route("api/v1/dashboard/phd-student/all-publications")]
    [Authorize(Roles = "PhdStudent")]
    public class PhdStudentLabPublicationsController : ControllerBase
    {
        private readonly IPublicationService _publicationService;

        public PhdStudentLabPublicationsController(IPublicationService publicationService)
        {
            _publicationService = publicationService;
        }

        // ── GET /dashboard/phd-student/all-publications ────────────────────────
        // Doctorants see all PUBLIE publications regardless of visibility.
        [HttpGet]
        public async Task<IActionResult> GetLabPublications(
            [FromQuery] string? search     = null,
            [FromQuery] string? type       = null,
            [FromQuery] string? ranking    = null,
            [FromQuery] int?    year       = null,
            [FromQuery] Guid?   axeId      = null,
            [FromQuery] string? visibility = null,
            [FromQuery] int     page       = 1,
            [FromQuery] int     limit      = 10,
            CancellationToken   ct         = default)
        {
            PublicationType? parsedType = null;
            if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<PublicationType>(type, true, out var t))
                parsedType = t;

            PublicationVisibility? parsedVisibility = null;
            if (!string.IsNullOrWhiteSpace(visibility) && Enum.TryParse<PublicationVisibility>(visibility, true, out var v))
                parsedVisibility = v;

            var (items, total) = await _publicationService.GetFilteredAsync(
                type:              parsedType,
                status:            PublicationStatus.Published,   // always only published
                visibility:        parsedVisibility,              // null = both public & private
                userId:            null,
                researchAxisId:    axeId,
                year:              year,
                search:            search,
                page:              page,
                pageSize:          limit,
                cancellationToken: ct);

            int totalPages = (int)Math.Ceiling(total / (double)limit);

            return Ok(new
            {
                data = items.Select(p => new
                {
                    id          = p.Id,
                    type        = p.Type.ToString(),
                    title       = p.Title,
                    year        = p.Year,
                    visibility  = p.Visibility.ToString(),
                    quartile    = p.JournalArticle?.Ranking.ToString(),
                    coreRanking = p.InternationalConference?.Ranking.ToString(),
                    authors     = p.Authors,
                    axe         = p.ResearchAxis?.Title,
                    abstract_   = p.Abstract,
                    doi         = p.Doi
                }),
                pagination = new { total, page, limit, totalPages }
            });
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Master Student — all lab publications (same logic as PhD student)
    // Route: /api/v1/dashboard/master-student/all-publications
    // ══════════════════════════════════════════════════════════════════════════

    [ApiController]
    [Route("api/v1/dashboard/master-student/all-publications")]
    [Authorize(Roles = "MasterStudent")]
    public class MasterStudentLabPublicationsController : ControllerBase
    {
        private readonly IPublicationService _publicationService;

        public MasterStudentLabPublicationsController(IPublicationService publicationService)
        {
            _publicationService = publicationService;
        }

        // ── GET /dashboard/master-student/all-publications ─────────────────────
        // Identical business logic to PhD student lab view.
        [HttpGet]
        public async Task<IActionResult> GetLabPublications(
            [FromQuery] string? search     = null,
            [FromQuery] string? type       = null,
            [FromQuery] string? ranking    = null,
            [FromQuery] int?    year       = null,
            [FromQuery] Guid?   axeId      = null,
            [FromQuery] string? visibility = null,
            [FromQuery] int     page       = 1,
            [FromQuery] int     limit      = 10,
            CancellationToken   ct         = default)
        {
            PublicationType? parsedType = null;
            if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<PublicationType>(type, true, out var t))
                parsedType = t;

            PublicationVisibility? parsedVisibility = null;
            if (!string.IsNullOrWhiteSpace(visibility) && Enum.TryParse<PublicationVisibility>(visibility, true, out var v))
                parsedVisibility = v;

            var (items, total) = await _publicationService.GetFilteredAsync(
                type:              parsedType,
                status:            PublicationStatus.Published,
                visibility:        parsedVisibility,
                userId:            null,
                researchAxisId:    axeId,
                year:              year,
                search:            search,
                page:              page,
                pageSize:          limit,
                cancellationToken: ct);

            int totalPages = (int)Math.Ceiling(total / (double)limit);

            return Ok(new
            {
                data = items.Select(p => new
                {
                    id          = p.Id,
                    type        = p.Type.ToString(),
                    title       = p.Title,
                    year        = p.Year,
                    visibility  = p.Visibility.ToString(),
                    quartile    = p.JournalArticle?.Ranking.ToString(),
                    coreRanking = p.InternationalConference?.Ranking.ToString(),
                    authors     = p.Authors,
                    axe         = p.ResearchAxis?.Title,
                    abstract_   = p.Abstract,
                    doi         = p.Doi
                }),
                pagination = new { total, page, limit, totalPages }
            });
        }
    }
}
