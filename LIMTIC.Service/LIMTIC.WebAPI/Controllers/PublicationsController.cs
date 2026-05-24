using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.DTOs.Publications;
using LIMTIC.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.API.Controllers
{
    /// <summary>
    /// Public-facing publication endpoints (no auth required).
    /// Backs PublicationsPage and PublicationDetailPage.
    /// </summary>
    [ApiController]
    [Route("api/v1/public/publications")]
    [AllowAnonymous]
    public class PublicationsController : ControllerBase
    {
        private readonly IPublicationService _publicationService;

        public PublicationsController(IPublicationService publicationService)
        {
            _publicationService = publicationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPublications(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null,
            [FromQuery] int? year = null,
            [FromQuery] Guid? axeId = null,
            CancellationToken ct = default)
        {
            PublicationType? parsedType = null;
            if (!string.IsNullOrWhiteSpace(type) &&
                Enum.TryParse<PublicationType>(type, ignoreCase: true, out var t))
                parsedType = t;

            var (items, total) = await _publicationService.GetFilteredAsync(
                type: parsedType,
                status: PublicationStatus.Published,
                visibility: PublicationVisibility.Public,
                userId: null,
                researchAxisId: axeId,
                year: year,
                search: search,
                page: page,
                pageSize: limit,
                cancellationToken: ct);

            var list = items.ToList();
            int journals = list.Count(p => p.Type == PublicationType.ArticleJournal);
            int conferences = list.Count(p =>
                p.Type is PublicationType.ConferenceInternational
                       or PublicationType.ConferenceNational);
            int totalPages = (int)Math.Ceiling(total / (double)limit);

            return Ok(new
            {
                data = list.Select(p => new
                {
                    id = p.Id,
                    type = p.Type.ToString(),
                    publicationType = p.Type.ToString(),
                    year = p.Year,
                    title = p.Title,
                    authors = p.Authors,
                    venue = p.Venue,
                    doi = p.Doi,
                    ranking = p.JournalRanking?.ToString(),
                    coreRanking = p.CoreRanking?.ToString(),
                    axe = new { id = p.ResearchAxisId, title = p.ResearchAxisName }
                }),
                stats = new { total, journals, conferences },
                pagination = new { total, page, limit, totalPages }
            });
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentPublications(
            [FromQuery] int limit = 3,
            CancellationToken ct = default)
        {
            var publications = await _publicationService.GetRecentPublicPublicationsAsync(limit, ct);

            return Ok(publications.Select(p => new
            {
                id = p.Id,
                type = p.Type.ToString(),
                publicationType = p.Type.ToString(),
                year = p.Year,
                title = p.Title,
                authors = p.Authors,
                venue = p.Venue,
                doi = p.Doi,
                ranking = p.JournalRanking?.ToString(),
                coreRanking = p.CoreRanking?.ToString()
            }));
        }
        
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPublication(Guid id, CancellationToken ct = default)
        {
            var p = await _publicationService.GetByIdAsync(id, ct);

            if (p is null ||
                p.Status != PublicationStatus.Published ||
                p.Visibility != PublicationVisibility.Public)
                return NotFound(new { error = "NOT_FOUND", message = "Publication introuvable." });

            return Ok(BuildPublicationDetail(p));
        }
        private static object BuildPublicationDetail(PublicationDto p) => new
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
            // Journal-specific
            journalName = p.JournalArticle?.JournalName,
            volume = p.JournalArticle?.Volume,
            number = p.JournalArticle?.Number,
            pages = p.JournalArticle?.Pages
                              ?? p.BookChapter?.Pages
                              ?? p.NationalConference?.Pages
                              ?? p.InternationalConference?.Pages,
            ranking = p.JournalArticle?.RankingLabel,
            // International conference-specific
            coreRanking = p.InternationalConference?.RankingLabel,
            location = p.InternationalConference?.Location
                              ?? p.NationalConference?.Location,
            // Book chapter-specific
            bookTitle = p.BookChapter?.BookTitle,
            publisher = p.BookChapter?.Publisher,
            isbn = p.BookChapter?.Isbn,
            // Technical report-specific
            reportNumber = p.TechnicalReport?.ReportNumber,
            institution = p.TechnicalReport?.Institution
        };
    }
}