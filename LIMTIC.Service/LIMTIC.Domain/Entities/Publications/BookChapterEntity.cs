using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Publications
{
    public class BookChapterEntity : BaseEntity
    {
        public PublicationEntity Publication { get; set; }

        public string BookTitle { get; set; } = string.Empty;        
        public string Publisher { get; set; } = string.Empty;
        public string? Isbn { get; set; }             
        public string? Pages { get; set; }              
    }
}
