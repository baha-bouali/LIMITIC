using LIMTIC.WebAPI.Models.Publications.Common;

namespace LIMTIC.WebAPI.Models.Publications.Dashboard.GetPublications
{
    public class DashboardPublicationsListResponse
    {
        public IEnumerable<DashboardPublicationSummaryResponse> Data { get; set; } = [];
        public PaginationResponse Pagination { get; set; } = new();
    }
}

