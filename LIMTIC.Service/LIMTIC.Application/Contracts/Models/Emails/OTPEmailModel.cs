namespace LIMTIC.Application.Contracts.Models.Emails
{
    public class OTPEmailModel
    {
        public string ToEmail { get; init; } = string.Empty;
        public string OTPCode { get; init; } = string.Empty;
        public int ExpiryMinutes { get; init; }
        public int CurrentYear { get; init; } = DateTime.UtcNow.Year;
    }
}
