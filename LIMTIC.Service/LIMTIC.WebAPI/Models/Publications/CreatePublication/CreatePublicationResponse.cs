using LIMTIC.Application.DTOs.Publications;

namespace LIMTIC.WebAPI.Models.Publications.CreatePublication
{
    public class CreatePublicationResponse : BaseResponse
    {
        public PublicationDto? PublicationDto { get; set; }
    }
}

