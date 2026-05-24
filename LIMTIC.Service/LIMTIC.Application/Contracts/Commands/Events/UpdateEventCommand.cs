using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Contracts.Commands.Events
{
    public class UpdateEventCommand
    {
        public Guid Id { get; set; }
        public EventType Type { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string? Program { get; set; }
        public Guid ResearchAxisId { get; set; }
    }
}
