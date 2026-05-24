namespace LIMTIC.Application.Emails.Models
{
    public class ContactEmailModel
    {
        public string ToEmail { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public string SenderEmail { get; init; } = string.Empty;
        public string Subject { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public string SentAt { get; init; } = string.Empty;
        public int CurrentYear { get; init; } = DateTime.UtcNow.Year;
    }
}