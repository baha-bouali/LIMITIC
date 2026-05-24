namespace LIMTIC.Application.DTOs.Events
{
    public class SpeakerDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Institution { get; set; }
        public string? Role { get; set; }
        public string? Subject { get; set; }
    }
}
