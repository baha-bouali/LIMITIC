using LIMTIC.Domain.Enums;

namespace LIMTIC.Domain.Entities.Publications
{
    public class JournalArticleEntity : PublicationEntity
    {
        public string JournalName { get; set; }
        public string Volume { get; set; }
        public string Number { get; set; }
        public string Pages { get; set; }
        public JournalRanking Ranking { get; set; }

        public string GetRankingLabel() => Ranking.ToString();
    }
}
