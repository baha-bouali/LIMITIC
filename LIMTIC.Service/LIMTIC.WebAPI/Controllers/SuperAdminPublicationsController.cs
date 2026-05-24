using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.DTOs.Publications;
using LIMTIC.Application.Mappings;
using LIMTIC.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.API.Controllers.Dashboard
{
    /// <summary>
    /// SuperAdmin / Admin full CRUD over all publications,
    /// including approve / reject / visibility toggle.
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
        [HttpGet]
        public async Task<IActionResult> GetPublications(
            [FromQuery] string? search = null,
            [FromQuery] string? type = null,
            [FromQuery] string? status = null,
            [FromQuery] string? visibility = null,
            [FromQuery] int? year = null,
            [FromQuery] Guid? axeId = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            CancellationToken ct = default)
        {
            PublicationType? parsedType = null;
            if (!string.IsNullOrWhiteSpace(type) &&
                Enum.TryParse<PublicationType>(type, true, out var t))
                parsedType = t;

            PublicationStatus? parsedStatus = null;
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<PublicationStatus>(status, true, out var s))
                parsedStatus = s;

            PublicationVisibility? parsedVisibility = null;
            if (!string.IsNullOrWhiteSpace(visibility) &&
                Enum.TryParse<PublicationVisibility>(visibility, true, out var v))
                parsedVisibility = v;

            var (items, total) = await _publicationService.GetFilteredAsync(
                type: parsedType,
                status: parsedStatus,
                visibility: parsedVisibility,
                userId: null,
                researchAxisId: axeId,
                year: year,
                search: search,
                page: page,
                pageSize: limit,
                cancellationToken: ct);

            int totalPages = (int)Math.Ceiling(total / (double)limit);

            return Ok(new
            {
                data = items.Select(p => new
                {
                    id = p.Id,
                    type = p.Type.ToString(),
                    title = p.Title,
                    year = p.Year,
                    status = p.Status.ToString(),
                    visibility = p.Visibility.ToString(),
                    quartile = p.JournalRanking?.ToString(),
                    coreRanking = p.CoreRanking?.ToString(),
                    authors = p.Authors,
                    axe = new
                    {
                        id = p.ResearchAxisId,
                        title = p.ResearchAxisName
                    },
                    submittedBy = p.UserFullName,
                    doi = p.Doi,
                    rejectionReason = (string?)null   // extend when field is added to entity
                }),
                pagination = new { total, page, limit, totalPages }
            });
        }

        // ── POST /dashboard/superadmin/publications ────────────────────────────
        // SuperAdmin-created publications skip the approval workflow.
        [HttpPost]
        public async Task<IActionResult> CreatePublication(
            [FromBody] CreatePublicationRequest request,
            CancellationToken ct = default)
        {
            var entity = request.ToEntity();

            var created = await _publicationService.CreateAsync(entity, ct);
            await _publicationService.SubmitAsync(created.Id, ct);
            await _publicationService.ApproveAsync(created.Id, ct);

            return StatusCode(201, new
            {
                message = "Publication créée avec succès",
                publication = new { id = created.Id, status = PublicationStatus.Published.ToString() }
            });
        }

        // ── PUT /dashboard/superadmin/publications/{id} ────────────────────────
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePublication(
            Guid id,
            [FromBody] UpdatePublicationRequest request,
            CancellationToken ct = default)
        {
            var entity = request.ToEntity(id);
            await _publicationService.UpdateAsync(entity, ct);
            return Ok(new { message = "Publication mise à jour avec succès" });
        }

        // ── PUT /dashboard/superadmin/publications/{id}/visibility ─────────────
        [HttpPut("{id:guid}/visibility")]
        public async Task<IActionResult> UpdateVisibility(
            Guid id,
            [FromBody] UpdateVisibilityRequest request,
            CancellationToken ct = default)
        {
            if (!Enum.TryParse<PublicationVisibility>(
                    request.Visibility, ignoreCase: true, out var parsedVisibility))
                return BadRequest(new
                {
                    error = "VALIDATION_ERROR",
                    message = "Valeur de visibilité invalide."
                });

            var updated = await _publicationService.UpdateVisibilityAsync(id, parsedVisibility, ct);

            return Ok(new
            {
                message = "Visibilité mise à jour",
                visibility = updated.Visibility.ToString()
            });
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
        [HttpPost("{id:guid}/validate")]
        public async Task<IActionResult> ValidatePublication(Guid id, CancellationToken ct = default)
        {
            var publication = await _publicationService.ApproveAsync(id, ct);
            return Ok(new
            {
                message = "Publication validée et publiée avec succès",
                publication = new { id = publication.Id, status = publication.Status.ToString() }
            });
        }

        // ── POST /dashboard/superadmin/publications/{id}/reject ───────────────
        [HttpPost("{id:guid}/reject")]
        public async Task<IActionResult> RejectPublication(
            Guid id,
            [FromBody] RejectPublicationRequest request,
            CancellationToken ct = default)
        {
            var publication = await _publicationService.RejectAsync(id, request.Reason, ct);
            return Ok(new
            {
                message = "Publication rejetée",
                publication = new { id = publication.Id, status = publication.Status.ToString() }
            });
        }

        // ── POST /dashboard/superadmin/publications/{id}/pdf ──────────────────
        [HttpPost("{id:guid}/pdf")]
        public async Task<IActionResult> AddPdf(
            Guid id,
            [FromBody] PdfRequest request,
            CancellationToken ct = default)
        {
            await _publicationService.AddPdfAsync(id, request.PdfUrl, ct);
            return Ok(new { message = "PDF uploadé avec succès", pdfUrl = request.PdfUrl });
        }

        // ── DELETE /dashboard/superadmin/publications/{id}/pdf ────────────────
        [HttpDelete("{id:guid}/pdf")]
        public async Task<IActionResult> RemovePdf(
            Guid id,
            [FromBody] PdfRequest request,
            CancellationToken ct = default)
        {
            await _publicationService.RemovePdfAsync(id, request.PdfUrl, ct);
            return Ok(new { message = "PDF supprimé" });
        }
    }
}