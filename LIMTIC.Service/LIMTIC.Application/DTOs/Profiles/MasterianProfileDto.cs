using LIMTIC.Application.DTOs.UserManagement;

namespace LIMTIC.Application.DTOs.Profiles
{
    public class MasterianProfileDto : UserDto
    {
        public string DissertationSubject { get; set; }
        public string Cohort { get; set; }
        public Guid? SupervisorId { get; set; }
        public string? SupervisorName { get; set; }
    }
}
