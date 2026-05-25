namespace LIMTIC.Application.Contracts.Commands.Publications
{
    public class CreateDashboardPublicationCommand
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

        public CreateJournalArticleCommandItem? JournalArticle { get; set; }
        public CreateTechnicalReportCommandItem? TechnicalReport { get; set; }
        public CreateBookChapterCommandItem? BookChapter { get; set; }
        public CreateNationalConferenceCommandItem? NationalConference { get; set; }
        public CreateInternationalConferenceCommandItem? InternationalConference { get; set; }
    }

    public class CreateJournalArticleCommandItem
    {
        public string JournalName { get; set; } = string.Empty;
        public string Volume { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Pages { get; set; } = string.Empty;
        public string Ranking { get; set; } = string.Empty;
    }

    public class CreateTechnicalReportCommandItem
    {
        public long ReportNumber { get; set; }
        public string Institution { get; set; } = string.Empty;
    }

    public class CreateBookChapterCommandItem
    {
        public string BookTitle { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string? Isbn { get; set; }
        public string? Pages { get; set; }
    }

    public class CreateNationalConferenceCommandItem
    {
        public string ConferenceName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Pages { get; set; }
    }

    public class CreateInternationalConferenceCommandItem
    {
        public string ConferenceName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Pages { get; set; }
        public string Ranking { get; set; } = string.Empty;
    }
}

