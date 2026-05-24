namespace LIMTIC.WebAPI.Models.Events.UpdateSpeaker
{
    public class UpdateSpeakerRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Institution { get; set; }
        public string? Role { get; set; }
        public string? Subject { get; set; }
    }
}
