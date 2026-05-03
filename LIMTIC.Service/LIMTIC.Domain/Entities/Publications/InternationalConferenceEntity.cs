using LIMTIC.Domain.Enums;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Publications
{
    public class InternationalConferenceEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid PublicationId { get; set; }
        public PublicationEntity Publication { get; set; }

        public string ConferenceName { get; set; }
        public string Location { get; set; }
        public string? Pages { get; set; }
        public CoreRanking Ranking { get; set; }

        public string GetRankingLabel() => Ranking.ToString();
    }
}
