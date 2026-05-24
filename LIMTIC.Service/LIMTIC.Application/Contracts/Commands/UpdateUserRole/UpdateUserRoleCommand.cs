using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Contracts.Commands.UpdateUserRole
{
    public class UpdateUserRoleCommand
    {
        public Guid UserId { get; set; }
        public UserRole Role { get; set; }

        // Researcher fields
        public string? Rank { get; set; }
        public string? Specialty { get; set; }
        public string? Office { get; set; }
        public string? PhoneNumber { get; set; }
        public List<Guid>? ResearchAxisIds { get; set; }

        // PhDStudent fields
        public int? EnrollmentYear { get; set; }

        // Masterian fields
        public string? Cohort { get; set; }
        public string? DissertationSubject { get; set; }
    }
}
