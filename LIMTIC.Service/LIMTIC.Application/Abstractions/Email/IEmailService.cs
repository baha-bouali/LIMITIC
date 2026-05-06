using LIMTIC.Application.Contracts.Models.Emails;

namespace LIMTIC.Application.Abstractions.Email
{
    public interface IEmailService
    {
        Task SendOTPEmailAsync(OTPEmailModel model);
    }
}
