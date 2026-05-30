using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.DTOs.Publications
{
    public class PublicationDto
    {
        public Guid? Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ResearchAxisId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Abstract { get; set; } = string.Empty;

        public List<string> Keywords { get; set; } = [];
        public List<string> Authors { get; set; } = [];
        public string? Doi { get; set; }

        public string? Venue { get; set; }

        public int Year { get; set; }

        public PublicationType Type { get; set; }

        public PublicationStatus Status { get; set; }

        public PublicationVisibility Visibility { get; set; }

        public JournalArticleDto? JournalArticle { get; set; }

        public TechnicalReportDto? TechnicalReport { get; set; }

        public BookChapterDto? BookChapter { get; set; }

        public NationalConferenceDto? NationalConference { get; set; }

        public InternationalConferenceDto? InternationalConference { get; set; }
    }
}