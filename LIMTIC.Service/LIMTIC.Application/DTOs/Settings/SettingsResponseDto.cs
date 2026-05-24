namespace LIMTIC.Application.DTOs.Settings
{
    public class SettingsResponseDto
    {
        public IdentityDto Identity { get; set; } = new IdentityDto();
        public SmtpResponseDto Smtp { get; set; } = new SmtpResponseDto();
    }
}
