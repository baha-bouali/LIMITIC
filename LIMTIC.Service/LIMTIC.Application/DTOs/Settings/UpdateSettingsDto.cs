namespace LIMTIC.Application.DTOs.Settings
{
    public class UpdateSettingsDto
    {
        public IdentityDto Identity { get; set; } = new IdentityDto();
        public SmtpUpdateDto Smtp { get; set; } = new SmtpUpdateDto();
    }
}
