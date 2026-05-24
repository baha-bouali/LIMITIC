using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.DTOs.Profiles
{
    public class MasterianProfileDto
    {
        // User fields
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }

        // Masterian-specific fields
        public string DissertationSubject { get; set; }
        public string Cohort { get; set; }
        public string? PhotoUrl { get; set; }
        public Guid? SupervisorId { get; set; }
        public string? SupervisorName { get; set; }
    }
}
