using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using LIMTIC.Application.Abstractions.Email;
using LIMTIC.Application.Settings;
using LIMTIC.Infrastructure.Utils;
using Microsoft.Extensions.Options;

    namespace LIMTIC.Infrastructure.Persistence
    {
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly OTPTokenSettings _otpTokenSettings;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            IOptions<OTPTokenSettings> otpTokenSettings)
        {
            _emailSettings = emailSettings.Value;
            _otpTokenSettings = otpTokenSettings.Value;
        }

        public async Task SendOTPEmailAsync(string toEmail, string otp)
        {
            var smtpClient = new SmtpClient(_emailSettings.SmtpHost)
            {
                Port = _emailSettings.SmtpPort,
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.Password),
                // AFTER
                EnableSsl = _emailSettings.EnableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = "Your LIMTIC Password Reset Code",
                Body = EmailTemplates.GetOTPEmailTemplate(otp, _otpTokenSettings.ExpireInMinutes),
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }

}
