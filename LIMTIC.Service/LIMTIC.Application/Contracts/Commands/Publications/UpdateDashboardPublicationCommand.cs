namespace LIMTIC.Application.Contracts.Commands.Publications
{
    public class UpdateDashboardPublicationCommand
    {
        public Guid Id { get; set; }
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

        public UpdateJournalArticleCommandItem? JournalArticle { get; set; }
        public UpdateTechnicalReportCommandItem? TechnicalReport { get; set; }
        public UpdateBookChapterCommandItem? BookChapter { get; set; }
        public UpdateNationalConferenceCommandItem? NationalConference { get; set; }
        public UpdateInternationalConferenceCommandItem? InternationalConference { get; set; }
    }

    public class UpdateJournalArticleCommandItem
    {
        public string JournalName { get; set; } = string.Empty;
        public string Volume { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Pages { get; set; } = string.Empty;
        public string Ranking { get; set; } = string.Empty;
    }

    public class UpdateTechnicalReportCommandItem
    {
        public long ReportNumber { get; set; }
        public string Institution { get; set; } = string.Empty;
    }

    public class UpdateBookChapterCommandItem
    {
        public string BookTitle { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string? Isbn { get; set; }
        public string? Pages { get; set; }
    }

    public class UpdateNationalConferenceCommandItem
    {
        public string ConferenceName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Pages { get; set; }
    }

    public class UpdateInternationalConferenceCommandItem
    {
        public string ConferenceName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Pages { get; set; }
        public string Ranking { get; set; } = string.Empty;
    }
}

