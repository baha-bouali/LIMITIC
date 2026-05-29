using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Contracts.Commands.Events
{
    public class CreateEventCommand
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string? Program { get; set; }
        public Guid ResearchAxisId { get; set; }
        public List<CreateSpeakerItemCommand>? Speakers { get; set; } = [];
    }

    public class CreateSpeakerItemCommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Institution { get; set; }
        public string Role { get; set; }
        public string? Subject { get; set; }
    }
}
