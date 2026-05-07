namespace LIMTIC.WebAPI.Models.Auth.VerifyResetCode
{
    public class VerifyResetCodeRequest
    {
        public string Email { get; set; }
        public string OtpToken { get; set; }
    }
}
