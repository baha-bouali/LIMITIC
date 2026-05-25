using LIMTIC.Application.Abstractions.Settings;
using LIMTIC.Application.DTOs.Settings;
using LIMTIC.WebAPI.Models.Settings.UpdateSettings;
using LIMTIC.WebAPI.Models.Settings.TestSmtp;
using LIMTIC.WebAPI.Models.Settings.GetSettings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> UpdateSettings(UpdateSettingsRequest request)
        {
            var dto = new UpdateSettingsDto
            {
                Identity = new IdentityDto
                {
                    LabName = request.Identity.LabName,
                    LabSlogan = request.Identity.LabSlogan,
                    ContactEmail = request.Identity.ContactEmail,
                    Address = request.Identity.Address,
                    Phone = request.Identity.Phone,
                    LogoUrl = request.Identity.LogoUrl
                },
                Smtp = new SmtpUpdateDto
                {
                    Host = request.Smtp.Host,
                    Port = request.Smtp.Port,
                    Username = request.Smtp.Username,
                    Password = request.Smtp.Password,
                    UseTls = request.Smtp.UseTls
                }
            };

            var result = await _settingsService.UpdateSettingsAsync(dto);
            if (result.Success)
                return Ok(new { Success = true });

            return BadRequest(new { Success = false, Message = result.Message });
        }

        [HttpPost("smtp/test")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> TestSmtp(TestSmtpRequest request)
        {
            var result = await _settingsService.TestSmtpAsync(request.TestEmail);
            if (result.Success)
                return Ok(new { Success = true, Message = "Email envoyé" });

            return StatusCode(503, new { Success = false, Message = "SMTP_CONNECTION_FAILED" });
        }
    }
}
