using LIMTIC.Application.Contracts.Commands.ForgetPassword;
using LIMTIC.Application.Contracts.Commands.Login;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.Contracts.Commands.ResetPassword;
using LIMTIC.Application.Contracts.Commands.VerifyResetCode;
using LIMTIC.Application.DTOs.Auth;

namespace LIMTIC.Application.Abstractions.Auth
{
    public interface IAuthService
    {
        Task<Result<LoginDto>> Login(LoginCommand command);
        Task<Result<LoginDto>> ValidateRefreshToken(string? refreshToken);
      
        Task<Result<string>> ForgetPasswordAsync(ForgetPasswordCommand command);
        Task<Result<string>> VerifyResetTokenAsync(VerifyResetCodeCommand command);
        Task<Result<bool>> ResetPasswordAsync(ResetPasswordCommand command);

        Task<Result<bool>> Logout(string? refreshToken);
    }
}
