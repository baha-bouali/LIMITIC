namespace LIMTIC.Application.Contracts.Commands.ResetPassword
{
    public class ResetPasswordCommand
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ResetToken { get; set; }


    }
}
