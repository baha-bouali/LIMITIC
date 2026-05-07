namespace LIMTIC.WebAPI.Models.Auth.ResetPassword
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ResetToken { get; set; }

    }
}
