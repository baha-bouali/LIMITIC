using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Users
{
    public class ResearcherEntity : BaseEntity
    {
        public UserEntity User { get; set; }

        public string Rank { get; set; }
        public string Specialty { get; set; }
        public string Office { get; set; }       
        public string PhoneNumber { get; set; }
        public string? Biography { get; set; }
        public string? Photo { get; set; }     
        public string? Orcid { get; set; }
        public string? GoogleScholar { get; set; }
        public string? ResearchGate { get; set; }
        public string? LinkedIn { get; set; }
    }
}
