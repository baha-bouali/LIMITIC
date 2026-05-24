using LIMTIC.Application.DTOs.Events;
using LIMTIC.WebAPI;

namespace LIMTIC.WebAPI.Models.Events.UpdateEvent
{
    public class UpdateEventResponse : BaseResponse
    {
        public EventDto? Event { get; set; }
    }
}
