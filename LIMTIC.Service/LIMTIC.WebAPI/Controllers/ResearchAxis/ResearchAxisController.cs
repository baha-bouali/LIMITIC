using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Publications;
using LIMTIC.Application.Contracts.Commands.ResearchAxis;
using LIMTIC.Application.DTOs.ResearchAxis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers.ResearchAxis
{
    [ApiController]
    [Route("api/research-axes")]
    public class ResearchAxisController : ControllerBase
    {
        private readonly IResearchAxisService _researchAxisService;
        private readonly IPublicationAttachmentsService _publicationAttachmentsService;

        public ResearchAxisController(
            IResearchAxisService researchAxisService,
            IPublicationAttachmentsService publicationAttachmentsService)
        {
            _researchAxisService = researchAxisService;
            _publicationAttachmentsService = publicationAttachmentsService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllResearchAxis()
        {
            var result = await _researchAxisService.GetAllAsync();
            return Ok(new BaseResponse<List<ResearchAxisDto>> { Success = true, Data = result.Data });
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetResearchAxisById(Guid id)
        {
            var result = await _researchAxisService.GetByIdAsync(id);
            if (result.Success)
                return Ok(new BaseResponse<ResearchAxisDto> { Success = true, Data = result.Data });

            return NotFound(new BaseResponse<ResearchAxisDto> { Success = false, Message = result.Message });
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> CreateResearchAxis([FromBody] CreateResearchAxisCommand command)
        {
            var result = await _researchAxisService.CreateAsync(command);
            if (result.Success)
                return Ok(new BaseResponse<ResearchAxisDto> { Success = true, Data = result.Data });

            if (result.ValidationErrors != null)
                return BadRequest(new BaseResponse<ResearchAxisDto>
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });

            return BadRequest(new BaseResponse<ResearchAxisDto> { Success = false, Message = result.Message });
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> UpdateResearchAxis(Guid id, [FromBody] UpdateResearchAxisCommand command)
        {
            var result = await _researchAxisService.UpdateAsync(command);
            if (result.Success)
                return Ok(new BaseResponse<ResearchAxisDto> { Success = true, Data = result.Data });

            if (result.ValidationErrors != null)
                return BadRequest(new BaseResponse<ResearchAxisDto>
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });

            return NotFound(new BaseResponse<ResearchAxisDto> { Success = false, Message = result.Message });
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> DeleteResearchAxis(Guid id)
        {
            var result = await _researchAxisService.DeleteAsync(id);
            if (result.Success)
                return Ok(new BaseResponse<bool> { Success = true });

            // 422 when publications still reference this axis
            if (result.Message?.Contains("publications") == true)
                return UnprocessableEntity(new BaseResponse<ResearchAxisDto> { Success = false, Message = result.Message });

            return NotFound(new BaseResponse<bool> { Success = false, Message = result.Message });
        }

        // ── Members ────────────────────────────────────────────────────────────────

        [HttpPost("{id:guid}/members")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> AddMemberToResearchAxis(Guid researchAxisid, Guid userId)
        {
            var result = await _researchAxisService.AddMemberAsync(researchAxisid, userId);
            if (result.Success)
                return Ok(new BaseResponse<bool> { Success = true });

            return BadRequest(new BaseResponse<bool> { Success = false, Message = result.Message });
        }

        [HttpDelete("{id:guid}/members/{userId:guid}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> RemoveMember(Guid researchAxisid, Guid userId)
        {
            var result = await _researchAxisService.RemoveMemberAsync(researchAxisid, userId);
            if (result.Success)
                return Ok(new BaseResponse<bool> { Success = true });

            return NotFound(new BaseResponse<bool> { Success = false, Message = result.Message });
        }
    }
}
