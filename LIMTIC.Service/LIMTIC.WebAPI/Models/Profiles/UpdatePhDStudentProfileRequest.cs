namespace LIMTIC.WebAPI.Models.Profiles
{
    public class UpdatePhDStudentProfileRequest
    {
        public string? ThesisSubject { get; set; }
        public int EnrollmentYear { get; set; }
        public string? PhotoUrl { get; set; }
        public Guid? SupervisorId { get; set; }
        public List<Guid>? ResearchAxisIds { get; set; }
    }
}
