using LIMTIC.Application.DTOs.Profiles;

namespace LIMTIC.WebAPI.Models.ResearchAxis
{
    public class ResearchAxisResponse : BaseResponse
    {
        public ResearchAxisDto? ResearchAxis { get; set; }
    }

    public class ResearchAxesListResponse : BaseResponse
    {
        public List<ResearchAxisDto> ResearchAxes { get; set; } = [];
    }
}
