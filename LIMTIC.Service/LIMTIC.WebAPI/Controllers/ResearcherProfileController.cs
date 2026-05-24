using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Profiles;
using LIMTIC.Application.Contracts.Commands.Profiles;
using LIMTIC.WebAPI.Models.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers
{
    [ApiController]
    [Route("api/profiles/researchers")]
    public class ResearcherProfileController : ControllerBase
    {
        private readonly IResearcherProfileService _researcherProfileService;
        private readonly ICurrentUserService _currentUserService;

        public ResearcherProfileController(
            IResearcherProfileService researcherProfileService,
            ICurrentUserService currentUserService)
        {
            _researcherProfileService = researcherProfileService;
            _currentUserService = currentUserService;
        }

        [HttpGet("{userId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfile(Guid userId)
        {
            var result = await _researcherProfileService.GetByUserIdAsync(userId);
            if (result.Success)
                return Ok(new ResearcherProfileResponse { Success = true, Profile = result.Data });

            return NotFound(new ResearcherProfileResponse
            {
                Success = false,
                Message = result.Message
            });
        }

        [HttpPut("{userId:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(Guid userId, UpdateResearcherProfileRequest request)
        {
            var currentUserId = _currentUserService.UserId;
            var isAdmin = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");

            if (!isAdmin && currentUserId != userId)
                return Forbid();

            var command = new UpdateResearcherProfileCommand
            {
                UserId = userId,
                Rank = request.Rank,
                Specialty = request.Specialty,
                Office = request.Office,
                PhoneNumber = request.PhoneNumber,
                Biography = request.Biography,
                Orcid = request.Orcid,
                GoogleScholar = request.GoogleScholar,
                ResearchGate = request.ResearchGate,
                LinkedIn = request.LinkedIn,
                ResearchAxisIds = request.ResearchAxisIds
            };

            var result = await _researcherProfileService.UpdateAsync(command);
            if (result.Success)
                return Ok(new ResearcherProfileResponse { Success = true, Profile = result.Data?.Profile });

            if (result.ValidationErrors != null)
                return BadRequest(new ResearcherProfileResponse
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });

            return NotFound(new ResearcherProfileResponse
            {
                Success = false,
                Message = result.Message
            });
        }

        [HttpDelete("{userId:guid}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> DeleteProfile(Guid userId)
        {
            var result = await _researcherProfileService.DeleteAsync(userId);
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
