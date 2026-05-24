using LIMTIC.Application.DTOs.Events;
using LIMTIC.WebAPI;

namespace LIMTIC.WebAPI.Models.Events.CreateEvent
{
    public class CreateEventResponse : BaseResponse
    {
        public EventDto? Event { get; set; }
    }
}
