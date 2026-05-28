using LIMTIC.Application.Abstractions.Email;
using LIMTIC.Application.Emails.Models;
using LIMTIC.Application.Settings;
using LIMTIC.Domain.Abstractions.Settings;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace LIMTIC.Infrastructure.Emails
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ITemplateRenderer _renderer;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IDataProtector _protector;

        public EmailService(
            IOptions<EmailSettings> settings,
            ITemplateRenderer renderer,
            ISettingsRepository settingsRepository,
            IDataProtectionProvider dataProtectionProvider)
        {
            _settings = settings.Value;
            _renderer = renderer;
            _settingsRepository = settingsRepository;
            _protector = dataProtectionProvider.CreateProtector("smtp-password");
        }

        public Task SendOTPEmailAsync(OTPEmailModel model)
        {
            var body = _renderer.Render("OTPEmail", model);
            return Deliver(model.ToEmail, "Your LIMTIC Password Reset Code", body);
        }

        public async Task<bool> TestSmtpAsync(string testEmail)
        {
            try
            {
                await Deliver(testEmail, "LIMTIC SMTP Test", $"This is a test message from {_settings.SenderName}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task SendContactEmailAsync(ContactEmailModel model)
        {
            var body = _renderer.Render("ContactEmail", model);
            return Deliver(model.ToEmail, $"[Contact] {model.Subject}", body);
        }

        // ─── How to add a new email ────────────────────────────────────────────
        // 1. Create a model in Application/Emails/Models/
        // 2. Create a template in Infrastructure/Emails/Templates/
        // 3. Add the method signature to IEmailService
        // 4. Implement it here following the exact same pattern:
        //
        // public Task SendWelcomeEmailAsync(WelcomeEmailModel model)
        // {
        //     var body = _renderer.Render("WelcomeEmail", model);
        //     return Deliver(model.ToEmail, "Welcome to LIMTIC!", body);
        // }
        // ──────────────────────────────────────────────────────────────────────

        private async Task Deliver(string to, string subject, string htmlBody)
        {
            // Prefer DB-backed LabSettings when available (single source of truth)
            var labSettings = await _settingsRepository.GetAsync();

            string host;
            int port;
            string username;
            string password;
            bool enableSsl;
            string fromEmail;
            string fromName;

            if (labSettings != null && !string.IsNullOrEmpty(labSettings.SmtpHost))
            {
                host = labSettings.SmtpHost;
                port = labSettings.SmtpPort;
                username = labSettings.SmtpUsername ?? string.Empty;
                password = string.Empty;
                if (!string.IsNullOrEmpty(labSettings.SmtpPasswordHash))
                {
                    try
                    {
                        var protectedBytes = Convert.FromBase64String(labSettings.SmtpPasswordHash);
                        var unprotectedBytes = _protector.Unprotect(protectedBytes);
                        password = System.Text.Encoding.UTF8.GetString(unprotectedBytes);
                    }
                    catch
                    {
                        // fallback to legacy stored value
                        password = labSettings.SmtpPasswordHash;
                    }
                }
                enableSsl = labSettings.SmtpUseTls;
                fromEmail = string.IsNullOrEmpty(labSettings.ContactEmail) ? username : labSettings.ContactEmail;
                fromName = labSettings.LabName;
            }
            else
            {
                // fallback to configured appsettings
                host = _settings.SmtpHost;
                port = _settings.SmtpPort;
                username = _settings.SenderEmail;
                password = _settings.Password;
                enableSsl = _settings.EnableSsl;
                fromEmail = _settings.SenderEmail;
                fromName = _settings.SenderName;
            }

            using var client = new SmtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mail.To.Add(to);
            await client.SendMailAsync(mail);
        }
    }
}