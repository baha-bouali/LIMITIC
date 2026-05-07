namespace LIMTIC.Application.Contracts.Commands.VerifyResetCode
{
   public class VerifyResetCodeCommand
    {
        public string Email { get; set; }
        public string OtpToken { get; set; }
    }
}
