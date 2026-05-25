using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Profiles;
using LIMTIC.Application.Contracts.Commands.Profiles;
using LIMTIC.WebAPI.Models.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers
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
            return Ok(new PhDStudentsListResponse { Success = true, Profiles = result.Data });
        }

        [HttpGet("{userId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfile(Guid userId)
        {
            var result = await _phDStudentProfileService.GetByUserIdAsync(userId);
            if (result.Success)
                return Ok(new PhDStudentProfileResponse { Success = true, Profile = result.Data });

            return NotFound(new PhDStudentProfileResponse
            {
                Success = false,
                Message = result.Message
            });
        }

        [HttpPut("{userId:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(Guid userId, UpdatePhDStudentProfileRequest request)
        {
            var currentUserId = _currentUserService.UserId;
            var isAdmin = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");

            if (!isAdmin && currentUserId != userId)
                return Forbid();

            var command = new UpdatePhDStudentProfileCommand
            {
                UserId = userId,
                ThesisSubject = request.ThesisSubject,
                EnrollmentYear = request.EnrollmentYear,
                SupervisorId = request.SupervisorId,
                ResearchAxisIds = request.ResearchAxisIds
            };

            var result = await _phDStudentProfileService.UpdateAsync(command);
            if (result.Success)
                return Ok(new PhDStudentProfileResponse { Success = true, Profile = result.Data?.Profile });

            if (result.ValidationErrors != null)
                return BadRequest(new PhDStudentProfileResponse
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });

            return NotFound(new PhDStudentProfileResponse
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
                return Ok(new BaseResponse { Success = true });

            return NotFound(new BaseResponse
            {
                Success = false,
                Message = result.Message
            });
        }
    }
}
