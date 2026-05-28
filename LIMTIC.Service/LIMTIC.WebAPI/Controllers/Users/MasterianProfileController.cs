using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Profiles;
using LIMTIC.Application.Contracts.Commands.Profiles;
using LIMTIC.Application.DTOs.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers.Users
{
    [ApiController]
    [Route("api/profiles/masterians")]
    public class MasterianProfileController : ControllerBase
    {
        private readonly IMasterianProfileService _masterianProfileService;
        private readonly ICurrentUserService _currentUserService;

        public MasterianProfileController(
            IMasterianProfileService masterianProfileService,
            ICurrentUserService currentUserService)
        {
            _masterianProfileService = masterianProfileService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProfiles()
        {
            var result = await _masterianProfileService.GetAllAsync();
            return Ok(new BaseResponse<List<MasterianProfileDto>> { Success = true, Data = result.Data });
        }

        [HttpGet("{userId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfile(Guid userId)
        {
            var result = await _masterianProfileService.GetByUserIdAsync(userId);
            if (result.Success)
                return Ok(new BaseResponse<MasterianProfileDto> { Success = true, Data = result.Data });

            return NotFound(new BaseResponse<MasterianProfileDto>
            {
                Success = false,
                Message = result.Message
            });
        }

        [HttpPut("{userId:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UpdateMasterianProfileCommand command)
        {
            var currentUserId = _currentUserService.UserId.Value;
            var isAdmin = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");

            if (!isAdmin && currentUserId != command.UserId)
                return Forbid();

            var result = await _masterianProfileService.UpdateAsync(command);
            if (result.Success)
                return Ok(new BaseResponse<MasterianProfileDto> { Success = true, Data = result.Data });

            if (result.ValidationErrors != null)
                return BadRequest(new BaseResponse<MasterianProfileDto>
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });

            return NotFound(new BaseResponse<MasterianProfileDto>
            {
                Success = false,
                Message = result.Message
            });
        }

        [HttpDelete("{userId:guid}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> DeleteProfile(Guid userId)
        {
            var result = await _masterianProfileService.DeleteAsync(userId);
            if (result.Success)
                return Ok(new BaseResponse<bool> { Success = true });

            return NotFound(new BaseResponse<bool>
            {
                Success = false,
                Message = result.Message
            });
        }
    }
}
