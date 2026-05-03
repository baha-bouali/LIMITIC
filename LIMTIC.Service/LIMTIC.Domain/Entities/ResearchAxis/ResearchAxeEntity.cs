using LIMTIC.Domain.Entities.Events;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.ResearchAxis
{
    public class ResearchAxisEntity : BaseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Themes { get; set; }

        public ICollection<PublicationEntity> Publications { get; set; } = [];
        public ICollection<EventEntity> Events { get; set; } = [];
    }
}
