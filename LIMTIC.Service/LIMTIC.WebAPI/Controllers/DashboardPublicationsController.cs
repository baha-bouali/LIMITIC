using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.DTOs.Publications;

using LIMTIC.Application.Mappings;
using LIMTIC.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.API.Controllers.Dashboard
{
    [ApiController]
    [Route("api/v1/publications")]
    [Authorize]
    public class DashboardPublicationsController : ControllerBase
    {
        private readonly IPublicationService _publicationService;
        private readonly ICurrentUserService _currentUserService;

        public DashboardPublicationsController(
            IPublicationService publicationService,
            ICurrentUserService currentUserService)
        {
            _publicationService = publicationService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPublications(
            [FromQuery] string scope = "mine",
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] string? type = null,
            [FromQuery] string? visibility = null,
            [FromQuery] int? year = null,
            [FromQuery] Guid? axeId = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            CancellationToken ct = default)
        {
            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

            if (scope == "all" && !isAdmin)
                return StatusCode(403, new
                {
                    error = "FORBIDDEN",
                    message = "Seuls les administrateurs peuvent accéder à toutes les publications."
                });

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

            Guid? scopedUserId = scope == "all" ? null : _currentUserService.UserId;

            var (items, total) = await _publicationService.GetFilteredAsync(
                type: parsedType,
                status: parsedStatus,
                visibility: parsedVisibility,
                userId: scopedUserId,
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
                    venue = p.Venue,
                    doi = p.Doi,
                    axe = new { id = p.ResearchAxisId, title = p.ResearchAxisName },
                    submittedBy = p.UserFullName,
                    rejectionReason = (string?)null
                }),
                pagination = new { total, page, limit, totalPages }
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePublication(
            [FromBody] CreatePublicationRequest request,
            CancellationToken ct = default)
        {
            var entity = request.ToEntity();
            entity.UserId = _currentUserService.UserId;

            var created = await _publicationService.CreateAsync(entity, ct);

            await _publicationService.SubmitAsync(created.Id, ct);

            bool canPublishDirectly =
                User.IsInRole("Researcher") ||
                User.IsInRole("Admin") ||
                User.IsInRole("SuperAdmin");

            if (canPublishDirectly)
            {
                var approved = await _publicationService.ApproveAsync(created.Id, ct);
                return StatusCode(201, new
                {
                    message = "Publication créée et publiée avec succès",
                    publication = new { id = approved.Id, status = approved.Status.ToString() }
                });
            }

            return StatusCode(201, new
            {
                message = "Publication soumise pour validation",
                publication = new { id = created.Id, status = PublicationStatus.Submitted.ToString() }
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPublication(Guid id, CancellationToken ct = default)
        {
            var p = await _publicationService.GetByIdAsync(id, ct);
            if (p is null)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            bool isAuthor = p.UserId == _currentUserService.UserId;

            if (!isAdmin && !isAuthor)
                return StatusCode(403, new
                {
                    error = "FORBIDDEN",
                    message = "Accès non autorisé à cette publication."
                });

            return Ok(new
            {
                id = p.Id,
                type = p.Type.ToString(),
                publicationType = p.Type.ToString(),
                status = p.Status.ToString(),
                visibility = p.Visibility.ToString(),
                title = p.Title,
                year = p.Year,
                authors = p.Authors,
                abstract_ = p.Abstract,
                keywords = p.Keywords,
                doi = p.Doi,
                venue = p.Venue,
                pdfUrl = p.AttachedPdfs.FirstOrDefault(),
                axe = new { id = p.ResearchAxisId, title = p.ResearchAxisName },
                submittedBy = p.UserFullName,
                rejectionReason = (string?)null,
                journalName = p.JournalArticle?.JournalName,
                volume = p.JournalArticle?.Volume,
                number = p.JournalArticle?.Number,
                pages = p.JournalArticle?.Pages
                                  ?? p.BookChapter?.Pages
                                  ?? p.NationalConference?.Pages
                                  ?? p.InternationalConference?.Pages,
                ranking = p.JournalArticle?.RankingLabel,
                coreRanking = p.InternationalConference?.RankingLabel,
                location = p.InternationalConference?.Location
                                  ?? p.NationalConference?.Location,
                bookTitle = p.BookChapter?.BookTitle,
                publisher = p.BookChapter?.Publisher,
                isbn = p.BookChapter?.Isbn,
                reportNumber = p.TechnicalReport?.ReportNumber,
                institution = p.TechnicalReport?.Institution
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePublication(
            Guid id,
            [FromBody] UpdatePublicationRequest request,
            CancellationToken ct = default)
        {
            var existing = await _publicationService.GetByIdAsync(id, ct);
            if (existing is null)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            bool isAuthor = existing.UserId == _currentUserService.UserId;

            if (!isAdmin && !isAuthor)
                return StatusCode(403, new
                {
                    error = "NOT_OWNER",
                    message = "Vous n'êtes pas propriétaire de cette publication."
                });

            if (!isAdmin &&
                existing.Status is not (PublicationStatus.Draft or PublicationStatus.Rejected))
                return StatusCode(403, new
                {
                    error = "PUBLICATION_NOT_EDITABLE",
                    message = "Les publications soumises ou publiées ne peuvent pas être modifiées."
                });

            var entity = request.ToEntity(id);
            await _publicationService.UpdateAsync(entity, ct);
            return Ok(new { message = "Publication mise à jour avec succès" });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePublication(Guid id, CancellationToken ct = default)
        {
            var existing = await _publicationService.GetByIdAsync(id, ct);
            if (existing is null)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            bool isAuthor = existing.UserId == _currentUserService.UserId;

            if (!isAdmin && !isAuthor)
                return StatusCode(403, new
                {
                    error = "NOT_OWNER",
                    message = "Vous n'êtes pas propriétaire de cette publication."
                });

            if (!isAdmin && existing.Status != PublicationStatus.Draft)
                return StatusCode(403, new
                {
                    error = "PUBLICATION_NOT_EDITABLE",
                    message = "Seuls les brouillons peuvent être supprimés."
                });

            await _publicationService.DeleteAsync(id, ct);
            return Ok(new { message = "Publication supprimée avec succès" });
        }

        [HttpPost("{id:guid}/pdf")]
        public async Task<IActionResult> AddPdf(
            Guid id,
            [FromBody] PdfRequest request,
            CancellationToken ct = default)
        {
            var existing = await _publicationService.GetByIdAsync(id, ct);
            if (existing is null)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            bool isAuthor = existing.UserId == _currentUserService.UserId;

            if (!isAdmin && !isAuthor)
                return StatusCode(403, new
                {
                    error = "NOT_OWNER",
                    message = "Vous n'êtes pas propriétaire de cette publication."
                });

            await _publicationService.AddPdfAsync(id, request.PdfUrl, ct);
            return Ok(new { message = "PDF uploadé avec succès", pdfUrl = request.PdfUrl });
        }

        [HttpDelete("{id:guid}/pdf")]
        public async Task<IActionResult> RemovePdf(
            Guid id,
            [FromBody] PdfRequest request,
            CancellationToken ct = default)
        {
            var existing = await _publicationService.GetByIdAsync(id, ct);
            if (existing is null)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            bool isAuthor = existing.UserId == _currentUserService.UserId;

            if (!isAdmin && !isAuthor)
                return StatusCode(403, new
                {
                    error = "NOT_OWNER",
                    message = "Vous n'êtes pas propriétaire de cette publication."
                });

            await _publicationService.RemovePdfAsync(id, request.PdfUrl, ct);
            return Ok(new { message = "PDF supprimé" });
        }

        [HttpPost("{id:guid}/submit")]
        public async Task<IActionResult> SubmitPublication(Guid id, CancellationToken ct = default)
        {
            var existing = await _publicationService.GetByIdAsync(id, ct);
            if (existing is null)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            if (existing.UserId != _currentUserService.UserId)
                return StatusCode(403, new
                {
                    error = "NOT_OWNER",
                    message = "Vous n'êtes pas propriétaire de cette publication."
                });

            var updated = await _publicationService.SubmitAsync(id, ct);
            return Ok(new
            {
                message = "Publication soumise pour validation",
                id = updated.Id,
                status = updated.Status.ToString()
            });
        }

        [HttpPost("{id:guid}/validate")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ValidatePublication(Guid id, CancellationToken ct = default)
        {
            var publication = await _publicationService.ApproveAsync(id, ct);
            return Ok(new
            {
                message = "Publication validée et publiée avec succès",
                id = publication.Id,
                status = publication.Status.ToString(),
                validatedBy = publication.UserFullName
            });
        }

        [HttpPost("{id:guid}/reject")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> RejectPublication(
            Guid id,
            [FromBody] RejectPublicationRequest request,
            CancellationToken ct = default)
        {
            var publication = await _publicationService.RejectAsync(id, request.Reason, ct);
            return Ok(new
            {
                message = "Publication rejetée",
                id = publication.Id,
                status = publication.Status.ToString(),
                rejectionReason = request.Reason
            });
        }
    }
}