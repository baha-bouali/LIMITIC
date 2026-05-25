using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.Contracts.Commands.Publications;
using LIMTIC.WebAPI.Models;
using LIMTIC.WebAPI.Models.Publications.Common;
using LIMTIC.WebAPI.Models.Publications.Public.GetPublication;
using LIMTIC.WebAPI.Models.Publications.Public.GetPublications;
using LIMTIC.WebAPI.Models.Publications.Public.GetRecent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers.Publications
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
            var command = new GetPublicPublicationsCommand
            {
                Page = page,
                Limit = limit,
                Search = search,
                Type = type,
                Year = year,
                AxeId = axeId,
                IsAuthenticated = User.Identity?.IsAuthenticated == true
            };

            var result = await _publicationService.GetPublicPublicationsAsync(command, ct);
            if (!result.Success)
                return BadRequest(new BaseResponse { Success = false, Message = result.Message });

            var response = new PublicationsListResponse
            {
                Data = result.Data!.Data.Select(p => new PublicPublicationSummaryResponse
                {
                    Id = p.Id,
                    Type = p.Type,
                    PublicationType = p.PublicationType,
                    Year = p.Year,
                    Title = p.Title,
                    Authors = p.Authors,
                    Venue = p.Venue,
                    Doi = p.Doi,
                    Ranking = p.Ranking,
                    CoreRanking = p.CoreRanking,
                    Axe = new PublicationAxeResponse { Id = p.Axe.Id, Title = p.Axe.Title }
                }).ToList(),
                Stats = new PublicationStatsResponse
                {
                    Total = result.Data.Stats.Total,
                    Journals = result.Data.Stats.Journals,
                    Conferences = result.Data.Stats.Conferences
                },
                Pagination = new PaginationResponse
                {
                    Total = result.Data.Pagination.Total,
                    Page = result.Data.Pagination.Page,
                    Limit = result.Data.Pagination.Limit,
                    TotalPages = result.Data.Pagination.TotalPages
                }
            };

            return Ok(response);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentPublications(
            [FromQuery] int limit = 3,
            CancellationToken ct = default)
        {
            var command = new GetRecentPublicPublicationsCommand { Limit = limit };
            var result = await _publicationService.GetRecentPublicPublicationsAsync(command, ct);
            if (!result.Success)
                return BadRequest(new BaseResponse { Success = false, Message = result.Message });

            var response = result.Data!.Select(p => new PublicPublicationCardResponse
            {
                Id = p.Id,
                Type = p.Type,
                PublicationType = p.PublicationType,
                Year = p.Year,
                Title = p.Title,
                Authors = p.Authors,
                Venue = p.Venue,
                Doi = p.Doi,
                Ranking = p.Ranking,
                CoreRanking = p.CoreRanking
            }).ToList();

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPublication(Guid id, CancellationToken ct = default)
        {
            var command = new GetPublicPublicationByIdCommand
            {
                Id = id,
                IsAuthenticated = User.Identity?.IsAuthenticated == true
            };

            var result = await _publicationService.GetPublicPublicationByIdAsync(command, ct);
            if (!result.Success)
                return NotFound(new ErrorResponse { Error = "NOT_FOUND", Message = result.Message ?? "Publication not found." });

            var p = result.Data!;
            var response = new PublicPublicationDetailResponse
            {
                Id = p.Id,
                Type = p.Type,
                PublicationType = p.PublicationType,
                Status = p.Status,
                Visibility = p.Visibility,
                Title = p.Title,
                Year = p.Year,
                Authors = p.Authors,
                Abstract_ = p.Abstract_,
                Keywords = p.Keywords,
                Doi = p.Doi,
                Venue = p.Venue,
                PdfUrl = p.PdfUrl,
                Axe = new PublicationAxeResponse { Id = p.Axe.Id, Title = p.Axe.Title },
                JournalName = p.JournalName,
                Volume = p.Volume,
                Number = p.Number,
                Pages = p.Pages,
                Ranking = p.Ranking,
                CoreRanking = p.CoreRanking,
                Location = p.Location,
                BookTitle = p.BookTitle,
                Publisher = p.Publisher,
                Isbn = p.Isbn,
                ReportNumber = p.ReportNumber,
                Institution = p.Institution
            };

            return Ok(response);
        }
    }
}
