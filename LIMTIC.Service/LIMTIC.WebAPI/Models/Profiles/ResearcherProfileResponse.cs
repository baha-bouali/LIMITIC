using LIMTIC.Application.DTOs.Profiles;

namespace LIMTIC.WebAPI.Models.Profiles
{
    public class ResearcherProfileResponse : BaseResponse
    {
        public ResearcherProfileDto? Profile { get; set; }
    }

    public class ResearchersListResponse : BaseResponse
    {
        public List<ResearcherProfileDto>? Profiles { get; set; }
    }
}
