using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.API.Controllers
{
    /// <summary>
    /// Public-facing publication endpoints consumed by the PublicationsPage
    /// and PublicationDetailPage (no auth required).
    /// Route: /api/v1/publications
    /// </summary>
    [ApiController]
    [Route("api/v1/publications")]
    [AllowAnonymous]
    public class PublicationsController : ControllerBase
    {
        private readonly IPublicationService _publicationService;

        public PublicationsController(IPublicationService publicationService)
        {
            _publicationService = publicationService;
        }

        // ── GET /publications ──────────────────────────────────────────────────
        // Used by: PublicationsPage — server-side filtered + paginated list.
        // Implicitly filters to status=Published & visibility=Public.
        [HttpGet]
        public async Task<IActionResult> GetPublications(
            [FromQuery] int?    page       = 1,
            [FromQuery] int?    limit      = 10,
            [FromQuery] string? search     = null,
            [FromQuery] string? type       = null,    // e.g. ARTICLE_JOURNAL
            [FromQuery] string? ranking    = null,    // e.g. q1, core-a-star
            [FromQuery] int?    year       = null,
            [FromQuery] Guid?   axeId      = null,
            CancellationToken   ct         = default)
        {
            PublicationType? parsedType = null;
            if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<PublicationType>(type, ignoreCase: true, out var t))
                parsedType = t;

            var (items, total) = await _publicationService.GetFilteredAsync(
                type:              parsedType,
                status:            PublicationStatus.Published,
                visibility:        PublicationVisibility.Public,
                userId:            null,
                researchAxisId:    axeId,
                year:              year,
                search:            search,
                page:              page ?? 1,
                pageSize:          limit ?? 10,
                cancellationToken: ct);

            // stats block: total public published publications, split by broad category
            var allPublic = items.ToList();
            int journals     = allPublic.Count(p => p.Type == PublicationType.ArticleJournal);
            int conferences  = allPublic.Count(p => p.Type is PublicationType.ConferenceInternational
                                                              or PublicationType.ConferenceNational);
            int totalPages   = (int)Math.Ceiling(total / (double)(limit ?? 10));

            return Ok(new
            {
                data       = allPublic.Select(p => new
                {
                    id              = p.Id,
                    type            = p.Type.ToString(),
                    publicationType = p.Type.ToString(),
                    year            = p.Year,
                    title           = p.Title,
                    authors         = p.Authors,
                    venue           = p.Venue,
                    doi             = p.Doi,
                    axe             = p.ResearchAxis == null ? null : new
                    {
                        id    = p.ResearchAxis.Id,
                        title = p.ResearchAxis.Title
                    }
                }),
                stats = new
                {
                    total       = total,
                    journals    = journals,
                    conferences = conferences
                },
                pagination = new
                {
                    total      = total,
                    page       = page ?? 1,
                    limit      = limit ?? 10,
                    totalPages = totalPages
                }
            });
        }

        // ── GET /publications/{id} ─────────────────────────────────────────────
        // Used by: PublicationDetailPage
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPublication(Guid id, CancellationToken ct = default)
        {
            var publication = await _publicationService.GetByIdAsync(id, ct);
            if (publication is null)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            // Only public published publications are accessible anonymously
            if (publication.Status != PublicationStatus.Published ||
                publication.Visibility != PublicationVisibility.Public)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            return Ok(BuildPublicationDetail(publication));
        }

        // ── GET /publications/recent ───────────────────────────────────────────
        // Used by: HomePage — "Publications Récentes" section
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentPublications(
            [FromQuery] int limit = 3,
            CancellationToken ct  = default)
        {
            var publications = await _publicationService.GetRecentPublicPublicationsAsync(limit, ct);
            return Ok(publications.Select(p => new
            {
                id              = p.Id,
                type            = p.Type.ToString(),
                publicationType = p.Type.ToString(),
                year            = p.Year,
                title           = p.Title,
                authors         = p.Authors,
                venue           = p.Venue,
                doi             = p.Doi
            }));
        }

        // ── POST /publications/{id}/pdf ────────────────────────────────────────
        // Handled by PdfController (see below) to keep this controller clean.

        // ─── Helpers ──────────────────────────────────────────────────────────

        private static object BuildPublicationDetail(
            LIMTIC.Domain.Entities.Publications.PublicationEntity p)
        {
            // Build the common base; type-specific fields are merged in below.
            var journal    = p.JournalArticle;
            var conference = p.InternationalConference ?? (object?)p.NationalConference;
            var chapter    = p.BookChapter;
            var report     = p.TechnicalReport;

            return new
            {
                id              = p.Id,
                type            = p.Type.ToString(),
                publicationType = p.Type.ToString(),
                status          = p.Status.ToString(),
                visibility      = p.Visibility.ToString(),
                title           = p.Title,
                year            = p.Year,
                authors         = p.Authors,
                abstract_       = p.Abstract,
                keywords        = p.Keywords,
                doi             = p.Doi,
                venue           = p.Venue,
                pdfUrl          = p.AttachedPdfs?.FirstOrDefault(),
                axe = p.ResearchAxis == null ? null : new
                {
                    id    = p.ResearchAxis.Id,
                    title = p.ResearchAxis.Title
                },
                // Journal-specific
                journalName    = journal?.JournalName,
                volume         = journal?.Volume,
                number         = journal?.Number,
                pages          = journal?.Pages ?? p.BookChapter?.Pages
                                               ?? p.NationalConference?.Pages
                                               ?? p.InternationalConference?.Pages,
                ranking        = journal?.Ranking.ToString(),
                // International conference-specific
                coreRanking    = p.InternationalConference?.Ranking.ToString(),
                location       = p.InternationalConference?.Location ?? p.NationalConference?.Location,
                // Book chapter-specific
                bookTitle      = p.BookChapter?.BookTitle,
                publisher      = p.BookChapter?.Publisher,
                isbn           = p.BookChapter?.Isbn,
                // Technical report-specific
                reportNumber   = p.TechnicalReport?.ReportNumber,
                institution    = p.TechnicalReport?.Institution
            };
        }
    }
}
