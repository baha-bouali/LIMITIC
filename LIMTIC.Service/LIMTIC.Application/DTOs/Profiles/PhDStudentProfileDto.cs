using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.DTOs.Profiles
{
    public class PhDStudentProfileDto
    {
        // User fields
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }

        // PhDStudent-specific fields
        public string? ThesisSubject { get; set; }
        public int EnrollmentYear { get; set; }
        public string? PhotoUrl { get; set; }
        public Guid? SupervisorId { get; set; }
        public string? SupervisorName { get; set; }
        public List<ResearchAxisDto> ResearchAxes { get; set; } = [];
    }
}
