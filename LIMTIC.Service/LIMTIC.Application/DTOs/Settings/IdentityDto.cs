namespace LIMTIC.Application.DTOs.Settings
{
    public class IdentityDto
    {
        public string LabName { get; set; } = string.Empty;
        public string LabSlogan { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
    }
}
