namespace LIMTIC.Application.DTOs.Settings
{
    public class SmtpUpdateDto
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        // Optional: only include when the user types a new password
        public string? Password { get; set; }
        public bool UseTls { get; set; }
    }
}
