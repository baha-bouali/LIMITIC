using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Events
{
    public class SpeakerEntity : BaseEntity
    {
        public Guid EventId { get; set; }
        public EventEntity Event { get; set; }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string? Institution { get; set; }
        public string? Role { get; set; }
        public string? Subject { get; set; }
    }
}
