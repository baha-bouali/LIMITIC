using LIMTIC.Domain.Entities.ResearchAxis;
using LIMTIC.Domain.Enums;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Events
{
    public class EventEntity : BaseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public EventType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string? Program { get; set; }  
        public List<string> PhotoFileNames { get; set; } = [];

        public EventStatus Status => DateTime.UtcNow < StartDate
            ? EventStatus.Upcoming
            : DateTime.UtcNow <= EndDate
                ? EventStatus.Ongoing
                : EventStatus.Past;

        public Guid ResearchAxisId { get; set; }
        public ResearchAxisEntity ResearchAxis { get; set; }

        public ICollection<SpeakerEntity> Speakers { get; set; } = [];
    }
}
