using LIMTIC.Domain.Enums;

namespace LIMTIC.WebAPI.Models.UserManagement.UpdateUserRole
{
    public class UpdateUserRoleRequest
    {
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
