using LIMTIC.Application.DTOs.Publications;

namespace LIMTIC.WebAPI.Models.Publications.GetPublication
{
    public class GetPublicationResponse : BaseResponse
    {
        public PublicationDto PublicationDto { get; set; }
    }
}

