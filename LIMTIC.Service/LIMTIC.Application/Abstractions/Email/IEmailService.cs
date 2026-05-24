using LIMTIC.Application.Emails.Models;

namespace LIMTIC.Application.Abstractions.Email
{
    public interface IEmailService
    {
        Task SendOTPEmailAsync(OTPEmailModel model);
        Task<bool> TestSmtpAsync(string testEmail);
    }
}
