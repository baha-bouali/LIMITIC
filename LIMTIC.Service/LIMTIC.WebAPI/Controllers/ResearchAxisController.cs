using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Contracts.Commands.ResearchAxis;
using LIMTIC.WebAPI.Models.ResearchAxis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers
{
    [ApiController]
    [Route("api/research-axes")]
    public class ResearchAxisController : ControllerBase
    {
        private readonly IResearchAxisService _researchAxisService;

        public ResearchAxisController(IResearchAxisService researchAxisService)
        {
            _researchAxisService = researchAxisService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var result = await _researchAxisService.GetAllAsync();
            return Ok(new ResearchAxesListResponse { Success = true, ResearchAxes = result.Data! });
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _researchAxisService.GetByIdAsync(id);
            if (result.Success)
                return Ok(new ResearchAxisResponse { Success = true, ResearchAxis = result.Data });

            return NotFound(new ResearchAxisResponse { Success = false, Message = result.Message });
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateResearchAxisRequest request)
        {
            var command = new CreateResearchAxisCommand
            {
                Title = request.Title,
                Description = request.Description,
                Themes = request.Themes,
                Color = request.Color,
                ResponsibleId = request.ResponsibleId,
                MemberIds = request.MemberIds
            };

            var result = await _researchAxisService.CreateAsync(command);
            if (result.Success)
                return Ok(new ResearchAxisResponse { Success = true, ResearchAxis = result.Data });

            if (result.ValidationErrors != null)
                return BadRequest(new ResearchAxisResponse
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });

            return BadRequest(new ResearchAxisResponse { Success = false, Message = result.Message });
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateResearchAxisRequest request)
        {
            var command = new UpdateResearchAxisCommand
            {
                Id = id,
                Title = request.Title,
                Description = request.Description,
                Themes = request.Themes,
                Color = request.Color,
                ResponsibleId = request.ResponsibleId,
                MemberIds = request.MemberIds
            };

            var result = await _researchAxisService.UpdateAsync(command);
            if (result.Success)
                return Ok(new ResearchAxisResponse { Success = true, ResearchAxis = result.Data });

            if (result.ValidationErrors != null)
                return BadRequest(new ResearchAxisResponse
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });

            return NotFound(new ResearchAxisResponse { Success = false, Message = result.Message });
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _researchAxisService.DeleteAsync(id);
            if (result.Success)
                return Ok(new BaseResponse { Success = true });

            // 422 when publications still reference this axis
            if (result.Message?.Contains("publications") == true)
                return UnprocessableEntity(new BaseResponse { Success = false, Message = result.Message });

            return NotFound(new BaseResponse { Success = false, Message = result.Message });
        }

        // ── Members ────────────────────────────────────────────────────────────────

        [HttpPost("{id:guid}/members")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> AddMember(Guid id, [FromBody] AddAxisMemberRequest request)
        {
            var result = await _researchAxisService.AddMemberAsync(id, request.UserId);
            if (result.Success)
                return Ok(new BaseResponse { Success = true });

            return BadRequest(new BaseResponse { Success = false, Message = result.Message });
        }

        [HttpDelete("{id:guid}/members/{userId:guid}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> RemoveMember(Guid id, Guid userId)
        {
            var result = await _researchAxisService.RemoveMemberAsync(id, userId);
            if (result.Success)
                return Ok(new BaseResponse { Success = true });

            return NotFound(new BaseResponse { Success = false, Message = result.Message });
        }
    }
}
