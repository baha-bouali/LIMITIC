using LIMTIC.Application.Abstractions.Settings;
using LIMTIC.Application.DTOs.Settings;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.Abstractions.Email;
using LIMTIC.Application.Helpers;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Enums;
using LIMTIC.Application.Abstractions;

namespace LIMTIC.Application.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IEmailService _emailService;
        private readonly Microsoft.AspNetCore.DataProtection.IDataProtector _protector;
        private readonly IAuditLogsRepository _auditLogsRepository;
        private readonly ICurrentUserService _currentUserService;

        public SettingsService(ISettingsRepository settingsRepository, IEmailService emailService, Microsoft.AspNetCore.DataProtection.IDataProtectionProvider dataProtectionProvider, IAuditLogsRepository auditLogsRepository, ICurrentUserService currentUserService)
        {
            _settingsRepository = settingsRepository;
            _emailService = emailService;
            _protector = dataProtectionProvider.CreateProtector("smtp-password");
            _auditLogsRepository = auditLogsRepository;
            _currentUserService = currentUserService;
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
            if (!updated)
                return Result<bool>.FailureResult("Failed to update settings");

            var settingsLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.UPDATE, ResourceType.Setting);
            await _auditLogsRepository.AddLog(settingsLog);

            return Result<bool>.SuccessResult(true);
        }

        public async Task<Result<bool>> TestSmtpAsync(string testEmail)
        {
            var ok = await _emailService.TestSmtpAsync(testEmail);
            return ok ? Result<bool>.SuccessResult(true) : Result<bool>.FailureResult("SMTP_CONNECTION_FAILED");
        }
    }
}
