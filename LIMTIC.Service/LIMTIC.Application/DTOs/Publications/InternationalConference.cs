using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.DTOs.Publications
{
    public sealed class InternationalConferenceDto
    {
        public string ConferenceName { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public CoreRanking Ranking { get; set; }
    }
}
