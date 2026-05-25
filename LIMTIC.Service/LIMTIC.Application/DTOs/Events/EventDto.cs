namespace LIMTIC.Application.DTOs.Events
{
    public class EventDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string? Program { get; set; }
        public List<string> PhotoFileNames { get; set; } = [];
        public List<SpeakerDto>? Speakers { get; set; } = new();
        public string ResearchAxisId { get; set; }
    }
}
