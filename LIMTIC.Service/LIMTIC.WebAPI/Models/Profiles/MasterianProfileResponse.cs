using LIMTIC.Application.DTOs.Profiles;

namespace LIMTIC.WebAPI.Models.Profiles
{
    public class MasterianProfileResponse : BaseResponse
    {
        public MasterianProfileDto? Profile { get; set; }
    }

    public class MastersListResponse : BaseResponse
    {
        public List<MasterianProfileDto>? Profiles { get; set; }
    }
}
