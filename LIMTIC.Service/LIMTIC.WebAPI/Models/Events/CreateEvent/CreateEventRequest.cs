namespace LIMTIC.WebAPI.Models.Events.CreateEvent
{
    public class CreateEventRequest
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string? Program { get; set; }
        public Guid ResearchAxisId { get; set; }
        public List<CreateEventSpeakerRequest>? Speakers { get; set; } = [];
    }

    public class CreateEventSpeakerRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Institution { get; set; }
        public string? Role { get; set; }
        public string? Subject { get; set; }
    }
}
