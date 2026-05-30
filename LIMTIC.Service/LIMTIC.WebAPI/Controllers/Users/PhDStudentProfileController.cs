using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Profiles;
using LIMTIC.Application.Contracts.Commands.Profiles;
using LIMTIC.Application.DTOs.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers.Users
{
    [ApiController]
    [Route("api/profiles/phd-students")]
    public class PhDStudentProfileController : ControllerBase
    {
        private readonly IPhDStudentProfileService _phDStudentProfileService;
        private readonly ICurrentUserService _currentUserService;

        public PhDStudentProfileController(
            IPhDStudentProfileService phDStudentProfileService,
            ICurrentUserService currentUserService)
        {
            _phDStudentProfileService = phDStudentProfileService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProfiles()
        {
            var result = await _phDStudentProfileService.GetAllAsync();
            return Ok(new BaseResponse<List<PhDStudentProfileDto>> { Success = true, Data = result.Data });
        }

        [HttpGet("{userId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfile(Guid userId)
        {
            var result = await _phDStudentProfileService.GetByUserIdAsync(userId);
            if (result.Success)
                return Ok(new BaseResponse<PhDStudentProfileDto> { Success = true, Data = result.Data });

            return NotFound(new BaseResponse<PhDStudentProfileDto>
            {
                Success = false,
                Message = result.Message
            });
        }

        [HttpPut("{userId:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UpdatePhDStudentProfileCommand command)
        {
            var currentUserId = _currentUserService.UserId.Value;
            var isAdmin = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");

            if (!isAdmin && currentUserId != command.UserId)
                return Forbid();

            var result = await _phDStudentProfileService.UpdateAsync(command);
            if (result.Success)
                return Ok(new BaseResponse<PhDStudentProfileDto> { Success = true, Data = result.Data });

            if (result.ValidationErrors != null)
                return BadRequest(new BaseResponse<PhDStudentProfileDto>
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });

            return NotFound(new BaseResponse<PhDStudentProfileDto>
            {
                Success = false,
                Message = result.Message
            });
        }

        [HttpDelete("{userId:guid}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> DeleteProfile(Guid userId)
        {
            var result = await _phDStudentProfileService.DeleteAsync(userId);
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
