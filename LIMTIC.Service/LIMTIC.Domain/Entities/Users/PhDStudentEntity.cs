using LIMTIC.Domain.Entities.ResearchAxis;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Users
{
    public class PhDStudentEntity : BaseEntity
    {
        public UserEntity User { get; set; }

        public string? ThesisSubject { get; set; }
        public int EnrollmentYear { get; set; }

        public Guid? SupervisorId { get; set; }
        public ResearcherEntity? Supervisor { get; set; }

        public ICollection<ResearchAxisEntity> ResearchAxes { get; set; } = [];
    }
}
