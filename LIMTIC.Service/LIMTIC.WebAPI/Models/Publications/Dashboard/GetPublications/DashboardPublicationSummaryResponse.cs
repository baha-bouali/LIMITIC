using LIMTIC.WebAPI.Models.Publications.Common;

namespace LIMTIC.WebAPI.Models.Publications.Dashboard.GetPublications
{
    public class DashboardPublicationSummaryResponse
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Visibility { get; set; } = string.Empty;
        public string? Quartile { get; set; }
        public string? CoreRanking { get; set; }
        public string[] Authors { get; set; } = [];
        public string? Venue { get; set; }
        public string? Doi { get; set; }
        public PublicationAxeResponse Axe { get; set; } = new();
        public string SubmittedBy { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
    }
}

