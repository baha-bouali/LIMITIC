namespace LIMTIC.Application.Abstractions.Email
{
    public interface ISmtpService
    {
        Task<bool> TestSmtpAsync(string testEmail);
    }
}
