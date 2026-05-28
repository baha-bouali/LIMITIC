using LIMTIC.Application.Abstractions.Publication;
using LIMTIC.Application.Contracts.Commands.Publications;
using LIMTIC.Application.Contracts.Queries.Publications;
using LIMTIC.Application.DTOs.Publications;
using LIMTIC.Application.DTOs.Storage;
using LIMTIC.WebAPI.Models;
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

        [HttpGet("{id:guid}/pdfs")]
         public async Task<IActionResult> GetPublicationPdfs(Guid publicationId)
        {
            var result = await _publicationService.GetPublicationPdfs(publicationId);

            if (result.Success)
                return Ok(new BaseResponse<List<FileDownloadDto>> { Success = true, Data = result.Data });

            return BadRequest(new BaseResponse<List<FileDownloadDto>> { Success = false, Message = result.Message });
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
                return Ok(new BaseResponse<bool>
                {
                    Success = true
                });

            return BadRequest(new BaseResponse<bool> { Success = false, Message = result.Message });
        }

        [HttpPost("{id:guid}/validate")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ValidatePublication(Guid publicationId)
        {
            var result = await _publicationService.ValidatePublicationAsync(publicationId);

            if (!result.Success)
                return BadRequest(new BaseResponse<bool> { Success = false, Message = result.Message });

            return Ok(new BaseResponse<bool>
            {
                Success = true
            });
        }

        [HttpPost("{id:guid}/reject")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> RejectPublication(Guid publicationId)
        {
            var result = await _publicationService.RejectPublicationAsync(publicationId);

            if (!result.Success)
                return BadRequest(new BaseResponse<bool> { Success = false, Message = result.Message });

            return Ok(new BaseResponse<bool>
            {
                Success = true
            });
        }
    }
}
