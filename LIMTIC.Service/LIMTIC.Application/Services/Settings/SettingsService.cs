using LIMTIC.Application.Abstractions.Settings;
using LIMTIC.Application.DTOs.Settings;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.Abstractions.Email;
using LIMTIC.Domain.Abstractions;

namespace LIMTIC.Application.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly ISmtpService _smtpService;
        private readonly Microsoft.AspNetCore.DataProtection.IDataProtector _protector;

        public SettingsService(ISettingsRepository settingsRepository, ISmtpService smtpService, Microsoft.AspNetCore.DataProtection.IDataProtectionProvider dataProtectionProvider)
        {
            _settingsRepository = settingsRepository;
            _smtpService = smtpService;
            _protector = dataProtectionProvider.CreateProtector("smtp-password");
        }

        public async Task<Result<SettingsResponseDto>> GetSettingsAsync()
        {
            var settings = await _settingsRepository.GetAsync();
            if (settings == null)
                return Result<SettingsResponseDto>.FailureResult("Settings not found");

            var response = new SettingsResponseDto
            {
                Identity = new IdentityDto
                {
                    LabName = settings.LabName,
                    LabSlogan = settings.LabSlogan,
                    ContactEmail = settings.ContactEmail,
                    Address = settings.Address,
                    Phone = settings.Phone,
                    LogoUrl = settings.LogoUrl
                },
                Smtp = new SmtpResponseDto
                {
                    Host = settings.SmtpHost,
                    Port = settings.SmtpPort,
                    Username = settings.SmtpUsername,
                    UseTls = settings.SmtpUseTls
                }
            };

            return Result<SettingsResponseDto>.SuccessResult(response);
        }

        public async Task<Result<bool>> UpdateSettingsAsync(UpdateSettingsDto dto)
        {
            var settings = await _settingsRepository.GetAsync();
            if (settings == null)
                return Result<bool>.FailureResult("Settings not found");

            // Identity
            settings.LabName = dto.Identity.LabName;
            settings.LabSlogan = dto.Identity.LabSlogan;
            settings.ContactEmail = dto.Identity.ContactEmail;
            settings.Address = dto.Identity.Address;
            settings.Phone = dto.Identity.Phone;
            settings.LogoUrl = dto.Identity.LogoUrl;

            // SMTP - only update password if provided
            settings.SmtpHost = dto.Smtp.Host;
            settings.SmtpPort = dto.Smtp.Port;
            settings.SmtpUsername = dto.Smtp.Username;
            if (!string.IsNullOrEmpty(dto.Smtp.Password))
            {
                // Protect SMTP password at rest
                try
                {
                    var protectedBytes = _protector.Protect(System.Text.Encoding.UTF8.GetBytes(dto.Smtp.Password));
                    settings.SmtpPasswordHash = Convert.ToBase64String(protectedBytes);
                }
                catch
                {
                    // Fall back to storing plain value if protection fails
                    settings.SmtpPasswordHash = dto.Smtp.Password;
                }
            }
            settings.SmtpUseTls = dto.Smtp.UseTls;

            var updated = await _settingsRepository.UpdateAsync(settings);
            return updated ? Result<bool>.SuccessResult(true) : Result<bool>.FailureResult("Failed to update settings");
        }

        public async Task<Result<bool>> TestSmtpAsync(string testEmail)
        {
            var ok = await _smtpService.TestSmtpAsync(testEmail);
            return ok ? Result<bool>.SuccessResult(true) : Result<bool>.FailureResult("SMTP_CONNECTION_FAILED");
        }
    }
}
