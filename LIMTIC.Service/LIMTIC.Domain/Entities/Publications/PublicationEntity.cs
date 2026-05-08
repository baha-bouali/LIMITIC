using LIMTIC.Domain.Entities.ResearchAxis;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Publications
{
    public abstract class PublicationEntity : BaseEntity
    {        
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public Guid ResearchAxisId { get; set; }
        public ResearchAxisEntity ResearchAxis { get; set; }

        public string? Title { get; set; }
        public string? Abstract { get; set; }
        public string[]? Keywords { get; set; }
        public string[]? AttachedPdfs { get; set; }
        public string? Doi { get; set; }
        public PublicationType Type { get; set; }
        public PublicationStatus Status { get; set; }
        public PublicationVisibility Visibility { get; set; }
        public int Year { get; set; }
    }
}
