using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.Interfaces.Services;
using LIMTIC.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers
{
    /// <summary>
    /// Researcher-scoped publication endpoints.
    /// The authenticated user may only read/write their own publications.
    /// Route: /api/v1/dashboard/researcher/publications
    /// </summary>
    [ApiController]
    [Route("api/v1/dashboard/researcher/publications")]
    [Authorize(Roles = "Researcher")]
    public class ResearcherPublicationsController : ControllerBase
    {
        private readonly IPublicationService _publicationService;
        private readonly ICurrentUserService _currentUserService;

        public ResearcherPublicationsController(
            IPublicationService publicationService,
            ICurrentUserService currentUserService)
        {
            _publicationService = publicationService;
            _currentUserService = currentUserService;
        }

        // ── GET /dashboard/researcher/publications ─────────────────────────────
        // Returns the current researcher's own publications, filtered + paginated.
        [HttpGet]
        public async Task<IActionResult> GetMyPublications(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] string? type   = null,
            [FromQuery] int?    year   = null,
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
                    id              = p.Id,
                    type            = p.Type.ToString(),
                    title           = p.Title,
                    status          = p.Status.ToString(),
                    quartile        = p.JournalArticle?.Ranking.ToString(),
                    coreRanking     = p.InternationalConference?.Ranking.ToString(),
                    year            = p.Year,
                    authors         = string.Join(", ", p.Authors),
                    venue           = p.Venue,
                    rejectionReason = (string?)null   // extend when field is added
                }),
                pagination = new { total, page, limit, totalPages }
            });
        }

        // ── POST /dashboard/researcher/publications ────────────────────────────
        // Researcher submits a new publication for admin review (BROUILLON → SOUMIS).
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
                message     = "Publication soumise pour validation",
                publication = new { id = created.Id, status = PublicationStatus.Submitted.ToString() }
            });
        }

        // ── PUT /dashboard/researcher/publications/{id} ────────────────────────
        // Only allowed when status is BROUILLON or REJETE.
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

        // ── DELETE /dashboard/researcher/publications/{id} ────────────────────
        // Only allowed when status is BROUILLON.
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

        // ── POST /dashboard/researcher/publications/{id}/submit ────────────────
        // Explicit submit action: BROUILLON → SOUMIS.
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

        // ── POST /dashboard/researcher/publications/{id}/pdf ───────────────────
        [HttpPost("{id:guid}/pdf")]
        public async Task<IActionResult> AddPdf(
            Guid id,
            [FromBody] ResearcherPdfRequest request,
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

        // ── DELETE /dashboard/researcher/publications/{id}/pdf ─────────────────
        [HttpDelete("{id:guid}/pdf")]
        public async Task<IActionResult> RemovePdf(
            Guid id,
            [FromBody] ResearcherPdfRequest request,
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

        // ─── Request DTOs ─────────────────────────────────────────────────────

        public record ResearcherPdfRequest(string PdfUrl);
    }
}
