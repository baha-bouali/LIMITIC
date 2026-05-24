using LIMTIC.Domain.Entities.Events;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.ResearchAxis
{
    public class ResearchAxisEntity : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Themes { get; set; }
        public string? Color { get; set; }

        public Guid? ResponsibleId { get; set; }
        public ResearcherEntity? Responsible { get; set; }

        public ICollection<PublicationEntity> Publications { get; set; } = [];
        public ICollection<EventEntity> Events { get; set; } = [];
        public ICollection<ResearcherEntity> Researchers { get; set; } = [];
        public ICollection<PhDStudentEntity> PhDStudents { get; set; } = [];
    }
}
