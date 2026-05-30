using LIMTIC.Application.Abstractions.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LIMTIC.Application.Contracts.Commands.Settings;
using LIMTIC.Application.DTOs.Settings;

namespace LIMTIC.WebAPI.Controllers.Settings
{
    [ApiController]
    [Route("dashboard/superadmin/settings")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetSettings()
        {
            var result = await _settingsService.GetSettingsAsync();
            if (!result.Success || result.Data == null)
                return BadRequest(new { Success = false, Message = result.Message });

            return Ok(new BaseResponse<SettingsResponseDto>
            {
                Success = true,
                Data = result.Data
            });
        }

        [HttpPut]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateSettings(SettingsCommand command)
        {
            var result = await _settingsService.UpdateSettingsAsync(command);
            if (result.Success)
                return Ok(new BaseResponse<bool> { Success = true });

            return BadRequest(new BaseResponse<bool> { Success = false, Message = result.Message });
        }

        [HttpPost("smtp/test")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> TestSmtp(string testEmail)
        {
            var result = await _settingsService.TestSmtpAsync(testEmail);
            if (result.Success)
                return Ok(new BaseResponse<bool> { Success = true, Message = "Email envoyé" });

            return StatusCode(503, new BaseResponse<bool> { Success = false, Message = "SMTP_CONNECTION_FAILED" });
        }
    }
}
