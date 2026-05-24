namespace LIMTIC.Application.Contracts.Commands.Profiles
{
    public class UpdatePhDStudentProfileCommand
    {
        public Guid UserId { get; set; }
        public string? ThesisSubject { get; set; }
        public int EnrollmentYear { get; set; }
        public string? PhotoUrl { get; set; }
        public Guid? SupervisorId { get; set; }
        public List<Guid>? ResearchAxisIds { get; set; }
    }
}
