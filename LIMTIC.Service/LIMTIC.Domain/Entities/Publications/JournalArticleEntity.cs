using LIMTIC.Domain.Enums;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Publications
{
    public class JournalArticleEntity : BaseEntity
    {
        public PublicationEntity Publication { get; set; }

        public string JournalName { get; set; }
        public string Volume { get; set; }
        public string Number { get; set; }
        public int Pages { get; set; }
        public JournalRanking Ranking { get; set; }
    }
}
