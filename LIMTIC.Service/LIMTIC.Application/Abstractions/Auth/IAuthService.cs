using LIMTIC.Application.Contracts.Commands.ForgetPassword;
using LIMTIC.Application.Contracts.Commands.Login;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.Contracts.Commands.ResetPassword;
using LIMTIC.Application.Contracts.Commands.VerifyResetCode;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Application.Abstractions.Auth
{
    public interface IAuthService
    {
        Task<Result<LoginCommandResponse>> Login(LoginCommand command);
        Task<Result<LoginCommandResponse>> ValidateRefreshToken(string? refreshToken);
      
        Task<Result<string>> ForgetPasswordAsync(ForgetPasswordCommand command);
        Task<Result<VerifyResetCodeCommandResponse>> VerifyResetTokenAsync(VerifyResetCodeCommand command);
        Task<Result<string>> ResetPasswordAsync(ResetPasswordCommand command);

        Task Logout(string? refreshToken);
    }
}
