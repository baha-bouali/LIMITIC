using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.DTOs.Publications
{
    public class JournalArticleDto
    {
        public string JournalName { get; set; }
        public string Volume { get; set; }
        public string Number { get; set; }
        public int Pages { get; set; }
        public JournalRanking Ranking { get; set; }
    }
}
