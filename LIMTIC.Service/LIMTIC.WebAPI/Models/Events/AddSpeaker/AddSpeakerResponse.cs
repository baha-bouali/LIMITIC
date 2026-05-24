using LIMTIC.Application.DTOs.Events;
using LIMTIC.WebAPI;

namespace LIMTIC.WebAPI.Models.Events.AddSpeaker
{
    public class AddSpeakerResponse : BaseResponse
    {
        public SpeakerDto? Speaker { get; set; }
    }
}
