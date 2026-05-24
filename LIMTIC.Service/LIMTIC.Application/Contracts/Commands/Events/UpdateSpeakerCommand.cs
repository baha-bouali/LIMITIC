namespace LIMTIC.Application.Contracts.Commands.Events
{
    public class UpdateSpeakerCommand
    {
        public Guid EventId { get; set; }
        public Guid SpeakerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Institution { get; set; }
        public string? Role { get; set; }
        public string? Biography { get; set; }
        public string? Photo { get; set; }
    }
}
