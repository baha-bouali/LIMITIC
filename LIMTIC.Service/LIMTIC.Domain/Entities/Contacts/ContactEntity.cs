using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Contacts
{
    public class ContactEntity : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SentAtUtc { get; set; }
    }
}
