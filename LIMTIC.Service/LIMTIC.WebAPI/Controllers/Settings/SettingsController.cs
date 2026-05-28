using LIMTIC.Application.Abstractions.Settings;
using LIMTIC.Application.DTOs.Settings;
using LIMTIC.WebAPI.Models.Settings.GetSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LIMTIC.Application.Contracts.Commands.Settings.SettingsCommand;
using static LIMTIC.Application.Contracts.Commands.Settings.SettingsCommands;

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

            var resp = new GetSettingsResponse
            {
                Success = true,
                Identity = new IdentityResponse
                {
                    LabName = result.Data.Identity.LabName,
                    LabSlogan = result.Data.Identity.LabSlogan,
                    ContactEmail = result.Data.Identity.ContactEmail,
                    Address = result.Data.Identity.Address,
                    Phone = result.Data.Identity.Phone,
                    LogoUrl = result.Data.Identity.LogoUrl
                },
                Smtp = new SmtpResponse
                {
                    Host = result.Data.Smtp.Host,
                    Port = result.Data.Smtp.Port,
                    Username = result.Data.Smtp.Username,
                    UseTls = result.Data.Smtp.UseTls
                }
            };

            return Ok(resp);
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
