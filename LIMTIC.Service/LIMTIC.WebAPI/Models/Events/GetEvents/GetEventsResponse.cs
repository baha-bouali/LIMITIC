using LIMTIC.Application.DTOs.Events;

namespace LIMTIC.WebAPI.Models.Events.GetEvents
{
    public class GetEventsResponse : BaseResponse
    {
        public List<EventDto>? Items { get; set; }
        public int Total { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
    }
}
