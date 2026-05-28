using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.Contracts.Commands.Publications;
using LIMTIC.Application.Contracts.Queries.Publications;
using LIMTIC.Application.DTOs.Publications;
using LIMTIC.WebAPI.Models;
using LIMTIC.WebAPI.Models.Publications.Dashboard.Pdf;
using LIMTIC.WebAPI.Models.Publications.Dashboard.Status;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers.Publications
{
    [ApiController]
    [Route("api/v1/publications")]
    public class PublicationsController : ControllerBase
    {
        private readonly IPublicationService _publicationService;

        public PublicationsController(
            IPublicationService publicationService)
        {
            _publicationService = publicationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPublications(GetPublicationsQuery query)
        {
            var result = await _publicationService.GetPublicationsAsync(query);
            if (!result.Success)
                return BadRequest(new BaseResponse<List<PublicationDto>> { Success = false, Message = result.Message });

            return Ok(new BaseResponse<List<PublicationDto>>
            {
                Success = true,
                Data = result.Data.Publications,
                Pagination = new PaginationResponse
                {
                    Page = query.Page,
                    Limit = query.Limit,
                    Total = result.Data.Total,
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePublication([FromBody] CreatePublicationCommand command)
        {
            var result = await _publicationService.CreatePublicationAsync(command);
            if (!result.Success)
                return BadRequest(new BaseResponse<PublicationDto> { Success = false, Message = result.Message });

            return Ok(new BaseResponse<PublicationDto>
            {
                Success = true,
                Data = result.Data
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPublication(Guid publicationId)
        {
            var result = await _publicationService.GetPublicationByIdAsync(publicationId);

            if (!result.Success)
            {
                return BadRequest(new BaseResponse<PublicationDto> { Success = false, Message = result.Message });
            }

            return Ok(new BaseResponse<PublicationDto>
            {
                Success = true,
                Data = result.Data
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePublication(UpdatePublicationCommand command)
        {
            var result = await _publicationService.UpdatePublicationAsync(command);
            if (result.Success)
                return Ok(new BaseResponse<bool> { Success = true });

            return BadRequest(new BaseResponse<bool> { Success = false, Message = result.Message });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePublication(Guid publicationId)
        {
            var result = await _publicationService.DeletePublicationAsync(publicationId);

            if (result.Success)
                return Ok(new BaseResponse<bool> { Success = true });

            return BadRequest(new BaseResponse<bool> { Success = false, Message = result.Message });
        }

        [HttpPost("{id:guid}/pdf")]
        public async Task<IActionResult> AddPublicationPdfs(AddPublicationPdfsCommand command)
        {
            var result = await _publicationService.AddPublicationPdfsAsync(command);

            if (result.Success)
                return Ok(new BaseResponse<bool> { Message = "PDFs uploaded successfully" });

            return BadRequest(new BaseResponse<bool> { Success = false, Message = result.Message });
        }

        [HttpDelete("{id:guid}/pdf")]
        public async Task<IActionResult> RemovePublicationPdfAsync(RemovePublicationPdfCommand command)
        {
            var result = await _publicationService.RemovePublicationPdfAsync(command);

            if (result.Success)
                return Ok(new BaseResponse<bool> { Success = true, Message = "PDF removed" });

            return BadRequest(new BaseResponse<bool> { Success = false, Message = result.Message });
        }

        [HttpPost("{id:guid}/submit")]
        public async Task<IActionResult> SubmitPublication(Guid publicationId)
        {
            var result = await _publicationService.SubmitPublicationAsync(publicationId);

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


        [HttpPost("/api/publications/{id:guid}/attachments")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UploadPublicationAttachments(Guid id, List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest(new BaseResponse { Success = false, Message = "No files provided" });

            var filePayloads = new List<(Stream Stream, string FileName)>();
            foreach (var file in files.Where(f => f != null && f.Length > 0))
                filePayloads.Add((file.OpenReadStream(), file.FileName));

            try
            {
                var result = await _publicationService.UploadAttachmentsAsync(id, filePayloads);
                if (!result.Success)
                    return BadRequest(new BaseResponse { Success = false, Message = result.Message });

                return Ok(new BaseResponse { Success = true });
            }
            finally
            {
                foreach (var payload in filePayloads)
                    payload.Stream.Dispose();
            }
        }

        [HttpGet("/api/publications/{id:guid}/attachments/{index:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicationAttachment(Guid id, int index)
        {
            var result = await _publicationAttachmentsService.GetAttachmentAsync(id, index);
            if (!result.Success || result.Data == null)
                return NotFound(new BaseResponse { Success = false, Message = result.Message });

            return File(result.Data.Stream, result.Data.ContentType, result.Data.FileName);
        }
    }
}
