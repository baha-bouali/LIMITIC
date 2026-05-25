using LIMTIC.WebAPI.Models.Publications.Common;

namespace LIMTIC.WebAPI.Models.Publications.Public.GetPublications
{
    public class PublicationsListResponse
    {
        public IEnumerable<PublicPublicationSummaryResponse> Data { get; set; } = [];
        public PublicationStatsResponse Stats { get; set; } = new();
        public PaginationResponse Pagination { get; set; } = new();
    }
}

