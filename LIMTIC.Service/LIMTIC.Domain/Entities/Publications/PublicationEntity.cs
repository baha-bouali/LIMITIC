using LIMTIC.Domain.Entities.Files;
using LIMTIC.Domain.Entities.ResearchAxis;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Publications
{
    public class PublicationEntity : BaseEntity
    {        
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public Guid ResearchAxisId { get; set; }
        public ResearchAxisEntity ResearchAxis { get; set; }

        public string Title { get; set; }
        public string Abstract { get; set; }
        public List<string> Authors { get; set; }
        public List<string> Keywords { get; set; }
        public int Year { get; set; }
        public string? Doi { get; set; }
        public string? Venue { get; set; }
        public PublicationType Type { get; set; }
        public PublicationStatus Status { get; set; }
        public PublicationVisibility Visibility { get; set; }

        public JournalArticleEntity? JournalArticle { get; set; }
        public TechnicalReportEntity? TechnicalReport { get; set; }
        public BookChapterEntity? BookChapter { get; set; }
        public NationalConferenceEntity? NationalConference { get; set; }
        public InternationalConferenceEntity? InternationalConference { get; set; }
        public ICollection<PublicationFileEntity> Files { get; set; } = new List<PublicationFileEntity>();
    }
}
