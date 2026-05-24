using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Users
{
    public class MasterianEntity : BaseEntity
    {
        public UserEntity User { get; set; }

        public string DissertationSubject { get; set; }
        public string Cohort { get; set; }
        public string? PhotoUrl { get; set; }

        public Guid? SupervisorId { get; set; }
        public ResearcherEntity? Supervisor { get; set; }
    }
}