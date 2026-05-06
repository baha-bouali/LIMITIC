using System.Net;
using System.Net.Mail;
using LIMTIC.Application.Abstractions.Email;
using LIMTIC.Application.Contracts.Models.Emails;
using LIMTIC.Application.Settings;
using Microsoft.Extensions.Options;

namespace LIMTIC.Infrastructure.Emails
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ITemplateRenderer _renderer;

        public EmailService(
            IOptions<EmailSettings> settings,
            ITemplateRenderer renderer)
        {
            _settings = settings.Value;
            _renderer = renderer;
        }

        public Task SendOTPEmailAsync(OTPEmailModel model)
        {
            var body = _renderer.Render("OTPEmail", model);
            return Deliver(model.ToEmail, "Your LIMTIC Password Reset Code", body);
        }

        // ─── How to add a new email ────────────────────────────────────────────
        // 1. Create a model in Application/Contracts/Models/Emails/
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
            using var client = new SmtpClient(_settings.SmtpHost)
            {
                Port = _settings.SmtpPort,
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mail.To.Add(to);
            await client.SendMailAsync(mail);
        }
    }
}