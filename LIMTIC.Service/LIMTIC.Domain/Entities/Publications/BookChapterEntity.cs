namespace LIMTIC.Domain.Entities.Publications
{
    public class BookChapterEntity : PublicationEntity
    {
        public string BookTitle { get; set; }           
        public string Publisher { get; set; }          
        public string? Isbn { get; set; }             
        public string? Pages { get; set; }              
    }
}
