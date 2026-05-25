namespace LIMTIC.WebAPI.Models.Publications.Dashboard.UpdatePublication
{
    public class UpdateDashboardPublicationRequest
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

        public UpdateJournalArticleRequestItem? JournalArticle { get; set; }
        public UpdateTechnicalReportRequestItem? TechnicalReport { get; set; }
        public UpdateBookChapterRequestItem? BookChapter { get; set; }
        public UpdateNationalConferenceRequestItem? NationalConference { get; set; }
        public UpdateInternationalConferenceRequestItem? InternationalConference { get; set; }
    }

    public class UpdateJournalArticleRequestItem
    {
        public string JournalName { get; set; } = string.Empty;
        public string Volume { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Pages { get; set; } = string.Empty;
        public string Ranking { get; set; } = string.Empty;
    }

    public class UpdateTechnicalReportRequestItem
    {
        public long ReportNumber { get; set; }
        public string Institution { get; set; } = string.Empty;
    }

    public class UpdateBookChapterRequestItem
    {
        public string BookTitle { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string? Isbn { get; set; }
        public string? Pages { get; set; }
    }

    public class UpdateNationalConferenceRequestItem
    {
        public string ConferenceName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Pages { get; set; }
    }

    public class UpdateInternationalConferenceRequestItem
    {
        public string ConferenceName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Pages { get; set; }
        public string Ranking { get; set; } = string.Empty;
    }
}

