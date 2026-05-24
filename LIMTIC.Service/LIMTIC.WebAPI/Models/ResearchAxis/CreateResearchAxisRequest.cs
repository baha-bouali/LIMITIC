namespace LIMTIC.WebAPI.Models.ResearchAxis
{
    public class CreateResearchAxisRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Themes { get; set; } = [];
        public string? Color { get; set; }
        public Guid? ResponsibleId { get; set; }
        public List<Guid>? MemberIds { get; set; }
    }
}
