using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Publications
{
    public class NationalConferenceEntity : PublicationEntity
    {
        public string ConferenceName { get; set; } 
        public string Location { get; set; }
        public string? Pages { get; set; }
    }
}
