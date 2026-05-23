using LIMTIC.Application.Abstractions.Email;
using LIMTIC.Domain.Abstractions;
using Microsoft.AspNetCore.DataProtection;
using System.Net;
using System.Net.Mail;

namespace LIMTIC.Infrastructure.Services
{
    public class SmtpService : ISmtpService
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IDataProtector _protector;

        public SmtpService(ISettingsRepository settingsRepository, IDataProtectionProvider dataProtectionProvider)
        {
            _settingsRepository = settingsRepository;
            _protector = dataProtectionProvider.CreateProtector("smtp-password");
        }

        public async Task<bool> TestSmtpAsync(string testEmail)
        {
            var settings = await _settingsRepository.GetAsync();
            if (settings == null)
                return false;

            try
            {
                var password = string.Empty;
                if (!string.IsNullOrEmpty(settings.SmtpPasswordHash))
                {
                    try
                    {
                        var protectedBytes = Convert.FromBase64String(settings.SmtpPasswordHash);
                        var unprotectedBytes = _protector.Unprotect(protectedBytes);
                        password = System.Text.Encoding.UTF8.GetString(unprotectedBytes);
                    }
                    catch
                    {
                        // if unprotect fails or value is not base64, fallback to stored value (legacy)
                        password = settings.SmtpPasswordHash;
                    }
                }

                using var client = new SmtpClient(settings.SmtpHost)
                {
                    Port = settings.SmtpPort,
                    EnableSsl = settings.SmtpUseTls,
                    Credentials = new NetworkCredential(settings.SmtpUsername ?? string.Empty, password ?? string.Empty)
                };

                using var mail = new MailMessage
                {
                    From = new MailAddress(string.IsNullOrEmpty(settings.ContactEmail) ? settings.SmtpUsername ?? "" : settings.ContactEmail, settings.LabName),
                    Subject = "LIMTIC SMTP Test",
                    Body = $"This is a test message from {settings.LabName}.",
                    IsBodyHtml = false
                };

                mail.To.Add(testEmail);
                await client.SendMailAsync(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
