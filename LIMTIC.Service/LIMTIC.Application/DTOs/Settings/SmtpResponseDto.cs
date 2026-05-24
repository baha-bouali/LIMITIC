namespace LIMTIC.Application.DTOs.Settings
{
    public class SmtpResponseDto
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool UseTls { get; set; }
    }
}
