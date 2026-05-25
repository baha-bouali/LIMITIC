using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.Contracts.Commands.Publications;
using LIMTIC.Domain.Enums;
using LIMTIC.WebAPI.Models.Publications.Common;
using LIMTIC.WebAPI.Models.Publications.Dashboard.CreatePublication;
using LIMTIC.WebAPI.Models.Publications.Dashboard.GetPublication;
using LIMTIC.WebAPI.Models.Publications.Dashboard.GetPublications;
using LIMTIC.WebAPI.Models.Publications.Dashboard.Pdf;
using LIMTIC.WebAPI.Models.Publications.Dashboard.Reject;
using LIMTIC.WebAPI.Models.Publications.Dashboard.UpdatePublication;
using LIMTIC.WebAPI.Models.Publications.Dashboard.Status;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers.Publications
{
    [ApiController]
    [Route("api/v1/publications")]
    [Authorize]
    public class DashboardPublicationsController : ControllerBase
    {
        private readonly IPublicationService _publicationService;

        public DashboardPublicationsController(
            IPublicationService publicationService)
        {
            _publicationService = publicationService;
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
            var command = new GetDashboardPublicationsCommand
            {
                Scope = scope,
                Search = search,
                Status = status,
                Type = type,
                Visibility = visibility,
                Year = year,
                AxeId = axeId,
                Page = page,
                Limit = limit
            };

            var result = await _publicationService.GetDashboardPublicationsAsync(command, ct);
            if (!result.Success)
                return BadRequest(new BaseResponse { Success = false, Message = result.Message });

            var response = new DashboardPublicationsListResponse
            {
                Data = result.Data!.Data.Select(p => new DashboardPublicationSummaryResponse
                {
                    Id = p.Id,
                    Type = p.Type,
                    Title = p.Title,
                    Year = p.Year,
                    Status = p.Status,
                    Visibility = p.Visibility,
                    Quartile = p.Quartile,
                    CoreRanking = p.CoreRanking,
                    Authors = p.Authors,
                    Venue = p.Venue,
                    Doi = p.Doi,
                    Axe = new PublicationAxeResponse { Id = p.Axe.Id, Title = p.Axe.Title },
                    SubmittedBy = p.SubmittedBy,
                    RejectionReason = p.RejectionReason
                }).ToList(),
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

        [HttpPost]
        public async Task<IActionResult> CreatePublication(
            [FromBody] CreateDashboardPublicationRequest request,
            CancellationToken ct = default)
        {
            var command = new CreateDashboardPublicationCommand
            {
                ResearchAxisId = request.ResearchAxisId,
                Title = request.Title,
                Abstract = request.Abstract,
                Keywords = request.Keywords,
                Doi = request.Doi,
                Venue = request.Venue,
                Type = request.Type,
                Visibility = request.Visibility,
                Year = request.Year,
                Authors = request.Authors,
                JournalArticle = request.JournalArticle is null ? null : new CreateJournalArticleCommandItem
                {
                    JournalName = request.JournalArticle.JournalName,
                    Volume = request.JournalArticle.Volume,
                    Number = request.JournalArticle.Number,
                    Pages = request.JournalArticle.Pages,
                    Ranking = request.JournalArticle.Ranking
                },
                TechnicalReport = request.TechnicalReport is null ? null : new CreateTechnicalReportCommandItem
                {
                    ReportNumber = request.TechnicalReport.ReportNumber,
                    Institution = request.TechnicalReport.Institution
                },
                BookChapter = request.BookChapter is null ? null : new CreateBookChapterCommandItem
                {
                    BookTitle = request.BookChapter.BookTitle,
                    Publisher = request.BookChapter.Publisher,
                    Isbn = request.BookChapter.Isbn,
                    Pages = request.BookChapter.Pages
                },
                NationalConference = request.NationalConference is null ? null : new CreateNationalConferenceCommandItem
                {
                    ConferenceName = request.NationalConference.ConferenceName,
                    Location = request.NationalConference.Location,
                    Pages = request.NationalConference.Pages
                },
                InternationalConference = request.InternationalConference is null ? null : new CreateInternationalConferenceCommandItem
                {
                    ConferenceName = request.InternationalConference.ConferenceName,
                    Location = request.InternationalConference.Location,
                    Pages = request.InternationalConference.Pages,
                    Ranking = request.InternationalConference.Ranking
                }
            };

            var result = await _publicationService.CreateDashboardPublicationAsync(command, ct);
            if (!result.Success)
                return BadRequest(new BaseResponse { Success = false, Message = result.Message });

            return StatusCode(201, new CreateDashboardPublicationResponse
            {
                Message = result.Data!.Message,
                Publication = new CreatedPublicationRef
                {
                    Id = result.Data.Id,
                    Status = result.Data.Status
                }
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPublication(Guid id, CancellationToken ct = default)
        {
            var result = await _publicationService.GetDashboardPublicationByIdAsync(
                new GetDashboardPublicationByIdCommand { Id = id }, ct);

            if (!result.Success)
            {
                if (string.Equals(result.Message, "Publication not found.", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new BaseResponse { Success = false, Message = result.Message ?? "Publication not found." });

                return StatusCode(403, new BaseResponse { Success = false, Message = result.Message ?? "Access denied." });
            }

            var detail = result.Data!;
            var p = detail.Publication;

            var response = new DashboardPublicationDetailResponse
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
                Institution = p.Institution,
                SubmittedBy = detail.SubmittedBy,
                RejectionReason = detail.RejectionReason
            };

            return Ok(response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePublication(
            Guid id,
            [FromBody] UpdateDashboardPublicationRequest request,
            CancellationToken ct = default)
        {
            var command = new UpdateDashboardPublicationCommand
            {
                Id = id,
                ResearchAxisId = request.ResearchAxisId,
                Title = request.Title,
                Abstract = request.Abstract,
                Keywords = request.Keywords,
                Doi = request.Doi,
                Venue = request.Venue,
                Type = request.Type,
                Visibility = request.Visibility,
                Year = request.Year,
                Authors = request.Authors,
                JournalArticle = request.JournalArticle is null ? null : new UpdateJournalArticleCommandItem
                {
                    JournalName = request.JournalArticle.JournalName,
                    Volume = request.JournalArticle.Volume,
                    Number = request.JournalArticle.Number,
                    Pages = request.JournalArticle.Pages,
                    Ranking = request.JournalArticle.Ranking
                },
                TechnicalReport = request.TechnicalReport is null ? null : new UpdateTechnicalReportCommandItem
                {
                    ReportNumber = request.TechnicalReport.ReportNumber,
                    Institution = request.TechnicalReport.Institution
                },
                BookChapter = request.BookChapter is null ? null : new UpdateBookChapterCommandItem
                {
                    BookTitle = request.BookChapter.BookTitle,
                    Publisher = request.BookChapter.Publisher,
                    Isbn = request.BookChapter.Isbn,
                    Pages = request.BookChapter.Pages
                },
                NationalConference = request.NationalConference is null ? null : new UpdateNationalConferenceCommandItem
                {
                    ConferenceName = request.NationalConference.ConferenceName,
                    Location = request.NationalConference.Location,
                    Pages = request.NationalConference.Pages
                },
                InternationalConference = request.InternationalConference is null ? null : new UpdateInternationalConferenceCommandItem
                {
                    ConferenceName = request.InternationalConference.ConferenceName,
                    Location = request.InternationalConference.Location,
                    Pages = request.InternationalConference.Pages,
                    Ranking = request.InternationalConference.Ranking
                }
            };

            var result = await _publicationService.UpdateDashboardPublicationAsync(command, ct);
            if (result.Success)
                return Ok(new BaseResponse { Success = true, Message = "Publication updated successfully" });

            if (string.Equals(result.Message, "Publication not found.", StringComparison.OrdinalIgnoreCase))
                return NotFound(new BaseResponse { Success = false, Message = result.Message ?? "Publication not found." });

            return StatusCode(403, new BaseResponse { Success = false, Message = result.Message ?? "Access denied." });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePublication(Guid id, CancellationToken ct = default)
        {
            var result = await _publicationService.DeleteDashboardPublicationAsync(
                new DeleteDashboardPublicationCommand { Id = id }, ct);

            if (result.Success)
                return Ok(new BaseResponse { Success = true, Message = "Publication deleted successfully" });

            if (string.Equals(result.Message, "Publication not found.", StringComparison.OrdinalIgnoreCase))
                return NotFound(new BaseResponse { Success = false, Message = result.Message ?? "Publication not found." });

            return StatusCode(403, new BaseResponse { Success = false, Message = result.Message ?? "Access denied." });
        }

        [HttpPost("{id:guid}/pdf")]
        public async Task<IActionResult> AddPdf(
            Guid id,
            [FromBody] PdfRequest request,
            CancellationToken ct = default)
        {
            var result = await _publicationService.AddDashboardPublicationPdfAsync(
                new AddDashboardPublicationPdfCommand { Id = id, PdfUrl = request.PdfUrl }, ct);

            if (result.Success)
                return Ok(new PdfUploadResponse { Message = "PDF uploaded successfully", PdfUrl = result.Data! });

            if (string.Equals(result.Message, "Publication not found.", StringComparison.OrdinalIgnoreCase))
                return NotFound(new BaseResponse { Success = false, Message = result.Message ?? "Publication not found." });

            return StatusCode(403, new BaseResponse { Success = false, Message = result.Message ?? "Access denied." });
        }

        [HttpDelete("{id:guid}/pdf")]
        public async Task<IActionResult> RemovePdf(
            Guid id,
            [FromBody] PdfRequest request,
            CancellationToken ct = default)
        {
            var result = await _publicationService.RemoveDashboardPublicationPdfAsync(
                new RemoveDashboardPublicationPdfCommand { Id = id, PdfUrl = request.PdfUrl }, ct);

            if (result.Success)
                return Ok(new PdfRemoveResponse { Message = "PDF removed" });

            if (string.Equals(result.Message, "Publication not found.", StringComparison.OrdinalIgnoreCase))
                return NotFound(new BaseResponse { Success = false, Message = result.Message ?? "Publication not found." });

            return StatusCode(403, new BaseResponse { Success = false, Message = result.Message ?? "Access denied." });
        }

        [HttpPost("{id:guid}/submit")]
        public async Task<IActionResult> SubmitPublication(Guid id, CancellationToken ct = default)
        {
            var result = await _publicationService.SubmitDashboardPublicationAsync(
                new SubmitDashboardPublicationCommand { Id = id }, ct);

            if (result.Success)
                return Ok(new PublicationStatusResponse
                {
                    Message = result.Data!.Message,
                    Id = result.Data.Id,
                    Status = result.Data.Status
                });

            if (string.Equals(result.Message, "Publication not found.", StringComparison.OrdinalIgnoreCase))
                return NotFound(new BaseResponse { Success = false, Message = result.Message ?? "Publication not found." });

            return StatusCode(403, new BaseResponse { Success = false, Message = result.Message ?? "Access denied." });
        }

        [HttpPost("{id:guid}/validate")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ValidatePublication(Guid id, CancellationToken ct = default)
        {
            var result = await _publicationService.ValidateDashboardPublicationAsync(
                new ValidateDashboardPublicationCommand { Id = id }, ct);

            if (!result.Success)
                return BadRequest(new BaseResponse { Success = false, Message = result.Message });

            return Ok(new PublicationValidatedResponse
            {
                Message = result.Data!.Message,
                Id = result.Data.Id,
                Status = result.Data.Status,
                ValidatedBy = result.Data.Extra
            });
        }

        [HttpPost("{id:guid}/reject")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> RejectPublication(
            Guid id,
            [FromBody] RejectPublicationRequest request,
            CancellationToken ct = default)
        {
            var result = await _publicationService.RejectDashboardPublicationAsync(
                new RejectDashboardPublicationCommand { Id = id, Reason = request.Reason }, ct);

            if (!result.Success)
                return BadRequest(new BaseResponse { Success = false, Message = result.Message });

            return Ok(new PublicationRejectedResponse
            {
                Message = result.Data!.Message,
                Id = result.Data.Id,
                Status = result.Data.Status,
                RejectionReason = result.Data.Extra
            });
        }
    }
}
