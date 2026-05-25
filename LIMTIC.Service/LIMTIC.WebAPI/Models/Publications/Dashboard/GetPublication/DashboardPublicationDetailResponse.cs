using LIMTIC.WebAPI.Models.Publications.Public.GetPublication;

namespace LIMTIC.WebAPI.Models.Publications.Dashboard.GetPublication
{
    public class DashboardPublicationDetailResponse : PublicPublicationDetailResponse
    {
        public string SubmittedBy { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
    }
}

