using LIMTIC.Domain.Entities.ResearchAxis;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Publications
{
    // Class representing an external author without a system account
    public class ExternalAuthor
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Institution { get; set; }
        public string? Email { get; set; }
    }

    public class PublicationEntity : BaseEntity
    {
        // Owner
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        // Research axis
        public Guid ResearchAxisId { get; set; }
        public ResearchAxisEntity ResearchAxis { get; set; }

        // Common fields
        public string Title { get; set; }
        public string Abstract { get; set; }
        public List<string> Keywords { get; set; } = [];
        public int Year { get; set; }
        public string? Doi { get; set; }
        public string? ExternalUrl { get; set; }
        public List<string> AttachedPdfs { get; set; }

        // Type
        public PublicationType Type { get; set; }

        // Workflow
        public PublicationStatus Status { get; set; }
        public PublicationVisibility Visibility { get; set; }

        // Venue
        public string? VenueName { get; set; }
        public string? Location { get; set; }
        public string? Publisher { get; set; }
        public string? Volume { get; set; }
        public string? Number { get; set; }
        public string? Pages { get; set; }

        // Rankings / Metrics
        public string? CoreRanking { get; set; }
        public string? ScimagoQuartile { get; set; }
        public double? ImpactFactor { get; set; }
        public double? Snip { get; set; }
        public int CitationCount { get; set; }
        public string? RankingSource { get; set; }

        // Type-specific optional fields
        public string? Isbn { get; set; }
        public long? ReportNumber { get; set; }

        // Approval
        public Guid? ApprovedByUserId { get; set; }
        public UserEntity? ApprovedByUser { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }

        // --- AUTHORS ---

        // Internal Authors (Many-to-Many Relationship mapped correctly in DB)
        public ICollection<PublicationInternalAuthorEntity> InternalAuthors { get; set; } = new List<PublicationInternalAuthorEntity>();

        // External Authors (Stored as JSON in the database)
        public List<ExternalAuthor> ExternalAuthors { get; set; } = [];
    }
}