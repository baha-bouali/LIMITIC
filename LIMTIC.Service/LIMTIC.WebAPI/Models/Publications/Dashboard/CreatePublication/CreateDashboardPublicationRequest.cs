namespace LIMTIC.WebAPI.Models.Publications.Dashboard.CreatePublication
{
    public class CreateDashboardPublicationRequest
    {
        public Guid ResearchAxisId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Abstract { get; set; } = string.Empty;
        public string[] Keywords { get; set; } = [];
        public string? Doi { get; set; }
        public string? Venue { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Visibility { get; set; } = string.Empty;
        public int Year { get; set; }
        public string[] Authors { get; set; } = [];

        public CreateJournalArticleRequestItem? JournalArticle { get; set; }
        public CreateTechnicalReportRequestItem? TechnicalReport { get; set; }
        public CreateBookChapterRequestItem? BookChapter { get; set; }
        public CreateNationalConferenceRequestItem? NationalConference { get; set; }
        public CreateInternationalConferenceRequestItem? InternationalConference { get; set; }
    }

    public class CreateJournalArticleRequestItem
    {
        public string JournalName { get; set; } = string.Empty;
        public string Volume { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Pages { get; set; } = string.Empty;
        public string Ranking { get; set; } = string.Empty;
    }

    public class CreateTechnicalReportRequestItem
    {
        public long ReportNumber { get; set; }
        public string Institution { get; set; } = string.Empty;
    }

    public class CreateBookChapterRequestItem
    {
        public string BookTitle { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string? Isbn { get; set; }
        public string? Pages { get; set; }
    }

    public class CreateNationalConferenceRequestItem
    {
        public string ConferenceName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Pages { get; set; }
    }

    public class CreateInternationalConferenceRequestItem
    {
        public string ConferenceName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Pages { get; set; }
        public string Ranking { get; set; } = string.Empty;
    }
}

