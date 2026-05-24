namespace LIMTIC.WebAPI.Models.Settings.GetSettings
{
    public class IdentityResponse
    {
        public string LabName { get; set; }
        public string LabSlogan { get; set; }
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string? LogoUrl { get; set; }
    }

    public class SmtpResponse
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public bool UseTls { get; set; }
    }

    public class GetSettingsResponse
    {
        public bool Success { get; set; }
        public IdentityResponse Identity { get; set; }
        public SmtpResponse Smtp { get; set; }
        public string? Message { get; set; }
    }
}
