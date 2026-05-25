using LIMTIC.Application.DTOs.Profiles;

namespace LIMTIC.WebAPI.Models.Profiles
{
    public class PhDStudentProfileResponse : BaseResponse
    {
        public PhDStudentProfileDto? Profile { get; set; }
    }

    public class PhDStudentsListResponse : BaseResponse
    {
        public List<PhDStudentProfileDto>? Profiles { get; set; }
    }
}
