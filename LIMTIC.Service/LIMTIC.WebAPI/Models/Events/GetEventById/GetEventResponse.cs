using LIMTIC.Application.DTOs.Events;

namespace LIMTIC.WebAPI.Models.Events.GetEventById
{
    public class GetEventResponse : BaseResponse
    {
        public EventDto? Event { get; set; }
    }
}