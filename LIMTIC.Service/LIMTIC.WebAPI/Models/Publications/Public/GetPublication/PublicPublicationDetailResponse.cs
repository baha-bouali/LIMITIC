using LIMTIC.WebAPI.Models.Publications.Common;

namespace LIMTIC.WebAPI.Models.Publications.Public.GetPublication
{
    public class PublicPublicationDetailResponse
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string PublicationType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Visibility { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public string[] Authors { get; set; } = [];
        public string Abstract_ { get; set; } = string.Empty;
        public string[] Keywords { get; set; } = [];
        public string? Doi { get; set; }
        public string? Venue { get; set; }
        public string? PdfUrl { get; set; }
        public PublicationAxeResponse Axe { get; set; } = new();

        public string? JournalName { get; set; }
        public string? Volume { get; set; }
        public string? Number { get; set; }
        public string? Pages { get; set; }
        public string? Ranking { get; set; }

        public string? CoreRanking { get; set; }
        public string? Location { get; set; }

        public string? BookTitle { get; set; }
        public string? Publisher { get; set; }
        public string? Isbn { get; set; }

        public long? ReportNumber { get; set; }
        public string? Institution { get; set; }
    }
}

