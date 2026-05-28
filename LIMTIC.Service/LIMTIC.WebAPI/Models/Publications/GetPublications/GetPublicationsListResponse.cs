using LIMTIC.Application.DTOs.Publications;

namespace LIMTIC.WebAPI.Models.Publications.GetPublications
{
    public class GetPublicationsListResponse
    {
        public IEnumerable<PublicationDto> Data { get; set; } = [];
        public PaginationResponse Pagination { get; set; } = new();
    }
}

