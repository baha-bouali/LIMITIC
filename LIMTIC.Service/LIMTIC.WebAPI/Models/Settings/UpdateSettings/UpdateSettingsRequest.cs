namespace LIMTIC.WebAPI.Models.Settings.UpdateSettings
{
    public class IdentityRequest
    {
        public string LabName { get; set; }
        public string LabSlogan { get; set; }
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string? LogoUrl { get; set; }
    }

    public class SmtpRequest
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string? Password { get; set; }
        public bool UseTls { get; set; }
    }

    public class UpdateSettingsRequest
    {
        public IdentityRequest Identity { get; set; }
        public SmtpRequest Smtp { get; set; }
    }
}
