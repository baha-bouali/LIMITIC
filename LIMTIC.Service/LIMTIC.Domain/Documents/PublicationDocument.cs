using LIMTIC.Domain.Enums;

namespace LIMTIC.Domain.Documents
{
    public class PublicationDocument
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Abstract { get; set; }

        public List<string> Keywords { get; set; } = [];

        public Guid AuthorId { get; set; }

        public string AuthorFullName { get; set; }

        public Guid ResearchAxisId { get; set; }

        public string ResearchAxisName { get; set; }

        public PublicationType Type { get; set; }

        public int Year { get; set; }

        public string? VenueName { get; set; }

        public string? Publisher { get; set; }

        public string? Location { get; set; }

        public string? CoreRanking { get; set; }

        public string? ScimagoQuartile { get; set; }

        public double? ImpactFactor { get; set; }

        public double? Snip { get; set; }

        public int CitationCount { get; set; }

        public string? Doi { get; set; }

        public string? PdfUrl { get; set; }

        public PublicationVisibility Visibility { get; set; }

        public DateTime PublishedAt { get; set; }
    }
}
