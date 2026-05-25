using LIMTIC.WebAPI.Models.Publications.Common;

namespace LIMTIC.WebAPI.Models.Publications.Public.GetPublications
{
    public class PublicPublicationSummaryResponse
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string PublicationType { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Title { get; set; } = string.Empty;
        public string[] Authors { get; set; } = [];
        public string? Venue { get; set; }
        public string? Doi { get; set; }
        public string? Ranking { get; set; }
        public string? CoreRanking { get; set; }
        public PublicationAxeResponse Axe { get; set; } = new();
    }
}

