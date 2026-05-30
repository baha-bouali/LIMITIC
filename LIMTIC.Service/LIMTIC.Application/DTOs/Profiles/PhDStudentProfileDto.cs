using LIMTIC.Application.DTOs.ResearchAxis;
using LIMTIC.Application.DTOs.UserManagement;

namespace LIMTIC.Application.DTOs.Profiles
{
    public class PhDStudentProfileDto : UserDto
    {
        public string? ThesisSubject { get; set; }
        public int EnrollmentYear { get; set; }
        public Guid? SupervisorId { get; set; }
        public string? SupervisorName { get; set; }
        public List<ResearchAxisDto> ResearchAxes { get; set; } = [];
    }
}
