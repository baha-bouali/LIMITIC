namespace LIMTIC.Application.Contracts.Commands.ResearchAxis
{
    public class CreateResearchAxisCommand
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Themes { get; set; } = [];
        public string? Color { get; set; }
        public Guid? ResponsibleId { get; set; }
        public List<Guid>? MemberIds { get; set; }
    }
}
