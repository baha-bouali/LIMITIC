using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Publications
{
    public class NationalConferenceEntity : BaseEntity
    {
        public PublicationEntity Publication { get; set; }

        public string ConferenceName { get; set; } 
        public string Location { get; set; }
        public int? Year { get; set; }

    }
}
