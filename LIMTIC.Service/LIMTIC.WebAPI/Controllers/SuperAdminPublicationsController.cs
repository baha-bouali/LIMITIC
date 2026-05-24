using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.Interfaces.Services;
using LIMTIC.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.API.Controllers.Dashboard
{
    /// <summary>
    /// SuperAdmin / Admin full CRUD over all publications, including
    /// approve / reject / visibility toggle.
    /// Route: /api/v1/dashboard/superadmin/publications
    /// </summary>
    [ApiController]
    [Route("api/v1/dashboard/superadmin/publications")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SuperAdminPublicationsController : ControllerBase
    {
        private readonly IPublicationService _publicationService;

        public SuperAdminPublicationsController(IPublicationService publicationService)
        {
            _publicationService = publicationService;
        }

        // ── GET /dashboard/superadmin/publications ─────────────────────────────
        // Full admin listing — all statuses, all visibilities, all filters.
        [HttpGet]
        public async Task<IActionResult> GetPublications(
            [FromQuery] string? search     = null,
            [FromQuery] string? type       = null,
            [FromQuery] string? status     = null,
            [FromQuery] string? visibility = null,
            [FromQuery] string? ranking    = null,
            [FromQuery] int?    year       = null,
            [FromQuery] Guid?   axeId      = null,
            [FromQuery] int     page       = 1,
            [FromQuery] int     limit      = 10,
            CancellationToken   ct         = default)
        {
            PublicationType? parsedType = null;
            if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<PublicationType>(type, true, out var t))
                parsedType = t;

            PublicationStatus? parsedStatus = null;
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<PublicationStatus>(status, true, out var s))
                parsedStatus = s;

            PublicationVisibility? parsedVisibility = null;
            if (!string.IsNullOrWhiteSpace(visibility) && Enum.TryParse<PublicationVisibility>(visibility, true, out var v))
                parsedVisibility = v;

            var (items, total) = await _publicationService.GetFilteredAsync(
                type:              parsedType,
                status:            parsedStatus,
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
                    id              = p.Id,
                    type            = p.Type.ToString(),
                    title           = p.Title,
                    year            = p.Year,
                    status          = p.Status.ToString(),
                    visibility      = p.Visibility.ToString(),
                    quartile        = p.JournalArticle?.Ranking.ToString(),
                    coreRanking     = p.InternationalConference?.Ranking.ToString(),
                    authors         = p.Authors,
                    axe             = p.ResearchAxis == null ? null : new
                    {
                        id    = p.ResearchAxis.Id,
                        title = p.ResearchAxis.Title
                    },
                    submittedBy     = p.User == null ? null
                                        : $"{p.User.FirstName} {p.User.LastName}",
                    doi             = p.Doi,
                    // rejectionReason would come from an audit log or dedicated field;
                    // surfaced here as null until that field is added to PublicationEntity.
                    rejectionReason = (string?)null
                }),
                pagination = new
                {
                    total      = total,
                    page,
                    limit,
                    totalPages
                }
            });
        }

        // ── POST /dashboard/superadmin/publications ────────────────────────────
        // Admin creates a publication directly (auto-published).
        [HttpPost]
        public async Task<IActionResult> CreatePublication(
            [FromBody] LIMTIC.Domain.Entities.Publications.PublicationEntity publication,
            CancellationToken ct = default)
        {
            var created = await _publicationService.CreateAsync(publication, ct);

            // SuperAdmin-created publications skip the approval workflow.
            await _publicationService.SubmitAsync(created.Id, ct);
            await _publicationService.ApproveAsync(created.Id, ct);

            return StatusCode(201, new
            {
                message     = "Publication créée avec succès",
                publication = new { id = created.Id, status = PublicationStatus.Published.ToString() }
            });
        }

        // ── PUT /dashboard/superadmin/publications/{id} ────────────────────────
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePublication(
            Guid id,
            [FromBody] LIMTIC.Domain.Entities.Publications.PublicationEntity publication,
            CancellationToken ct = default)
        {
            publication.Id = id;
            await _publicationService.UpdateAsync(publication, ct);
            return Ok(new { message = "Publication mise à jour avec succès" });
        }

        // ── PUT /dashboard/superadmin/publications/{id}/visibility ─────────────
        // Toggle public / private without touching other fields.
        [HttpPut("{id:guid}/visibility")]
        public async Task<IActionResult> UpdateVisibility(
            Guid id,
            [FromBody] UpdateVisibilityRequest request,
            CancellationToken ct = default)
        {
            var publication = await _publicationService.GetByIdAsync(id, ct);
            if (publication is null)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            if (!Enum.TryParse<PublicationVisibility>(request.Visibility, ignoreCase: true, out var parsedVisibility))
                return BadRequest(new { error = "VALIDATION_ERROR", message = "Valeur de visibilité invalide." });

            publication.Visibility = parsedVisibility;
            await _publicationService.UpdateAsync(publication, ct);

            return Ok(new { message = "Visibilité mise à jour", visibility = parsedVisibility.ToString() });
        }

        // ── DELETE /dashboard/superadmin/publications/{id} ────────────────────
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeletePublication(Guid id, CancellationToken ct = default)
        {
            var publication = await _publicationService.GetByIdAsync(id, ct);
            if (publication is null)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            await _publicationService.DeleteAsync(id, ct);
            return Ok(new { message = "Publication supprimée avec succès" });
        }

        // ── POST /dashboard/superadmin/publications/{id}/validate ──────────────
        // Approve a SOUMIS publication → PUBLIE.
        [HttpPost("{id:guid}/validate")]
        public async Task<IActionResult> ValidatePublication(Guid id, CancellationToken ct = default)
        {
            var publication = await _publicationService.ApproveAsync(id, ct);
            return Ok(new
            {
                message     = "Publication validée et publiée avec succès",
                publication = new { id = publication.Id, status = publication.Status.ToString() }
            });
        }

        // ── POST /dashboard/superadmin/publications/{id}/reject ───────────────
        // Reject a SOUMIS publication → REJETE.
        [HttpPost("{id:guid}/reject")]
        public async Task<IActionResult> RejectPublication(
            Guid id,
            [FromBody] RejectPublicationRequest request,
            CancellationToken ct = default)
        {
            var publication = await _publicationService.RejectAsync(id, request.Reason, ct);
            return Ok(new
            {
                message     = "Publication rejetée",
                publication = new { id = publication.Id, status = publication.Status.ToString() }
            });
        }

        // ── POST /publications/{id}/pdf ────────────────────────────────────────
        // Attach a CDN-stored PDF URL to a publication.
        [HttpPost("{id:guid}/pdf")]
        public async Task<IActionResult> AddPdf(
            Guid id,
            [FromBody] AddPdfRequest request,
            CancellationToken ct = default)
        {
            var publication = await _publicationService.AddPdfAsync(id, request.PdfUrl, ct);
            return Ok(new
            {
                message = "PDF uploadé avec succès",
                pdfUrl  = request.PdfUrl
            });
        }

        // ── DELETE /publications/{id}/pdf ──────────────────────────────────────
        [HttpDelete("{id:guid}/pdf")]
        public async Task<IActionResult> RemovePdf(
            Guid id,
            [FromBody] RemovePdfRequest request,
            CancellationToken ct = default)
        {
            await _publicationService.RemovePdfAsync(id, request.PdfUrl, ct);
            return Ok(new { message = "PDF supprimé" });
        }

        // ─── Request DTOs ─────────────────────────────────────────────────────

        public record UpdateVisibilityRequest(string Visibility);
        public record RejectPublicationRequest(string? Reason);
        public record AddPdfRequest(string PdfUrl);
        public record RemovePdfRequest(string PdfUrl);
    }
}
