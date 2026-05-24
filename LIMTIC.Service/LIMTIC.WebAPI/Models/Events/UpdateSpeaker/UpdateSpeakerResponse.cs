using LIMTIC.Application.DTOs.Events;
using LIMTIC.WebAPI;

namespace LIMTIC.WebAPI.Models.Events.UpdateSpeaker
{
    public class UpdateSpeakerResponse : BaseResponse
    {
        public SpeakerDto? Speaker { get; set; }
    }
}
