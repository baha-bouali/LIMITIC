namespace LIMTIC.WebAPI.Models.ResearchAxis
{
    public class UpdateResearchAxisRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Themes { get; set; } = [];
    }
}
