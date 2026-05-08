using LIMTIC.Domain.Enums;

namespace LIMTIC.Domain.Models.Publications
{
    public class PublicationSearchDocument
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? UserFullName { get; set; }
        public Guid ResearchAxisId { get; set; }
        public string? ResearchAxisTitle { get; set; }
        public string? Title { get; set; }
        public string? Abstract { get; set; }
        public string[]? Keywords { get; set; }
        public string? Doi { get; set; }
        public PublicationType Type { get; set; }
        public PublicationStatus Status { get; set; }
        public PublicationVisibility Visibility { get; set; }
        public int Year { get; set; }
    }
}