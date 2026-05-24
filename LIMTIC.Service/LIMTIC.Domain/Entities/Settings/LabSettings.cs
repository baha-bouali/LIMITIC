using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Settings
{
    public class LabSettings : BaseEntity
    {
        public string LabName { get; set; } = string.Empty;
        public string LabSlogan { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }

        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPasswordHash { get; set; } = string.Empty;
        public bool SmtpUseTls { get; set; }

        public static LabSettings CreateDefault()
        {
            return new LabSettings
            {
                LabName = "LIMTIC Lab",
                LabSlogan = string.Empty,
                ContactEmail = "",
                Address = string.Empty,
                Phone = string.Empty,
                LogoUrl = null,
                SmtpHost = string.Empty,
                SmtpPort = 25,
                SmtpUsername = string.Empty,
                SmtpPasswordHash = string.Empty,
                SmtpUseTls = false
            };
        }
    }
}
