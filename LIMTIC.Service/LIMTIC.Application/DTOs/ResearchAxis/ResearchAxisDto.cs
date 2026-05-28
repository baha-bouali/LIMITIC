using LIMTIC.Application.DTOs.Profiles;

namespace LIMTIC.Application.DTOs.ResearchAxis
{
    public class ResearchAxisDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Themes { get; set; } = [];
        public string? Color { get; set; }
        public Guid? ResponsibleId { get; set; }
        public string? ResponsibleName { get; set; }
        public int PublicationsCount { get; set; }
        public List<AxisMemberDto> Members { get; set; } = [];
    }
}
