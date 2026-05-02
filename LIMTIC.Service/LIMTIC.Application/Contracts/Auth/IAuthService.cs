using LIMTIC.Application.Commands.Login;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Application.Contracts.Auth
{
    public interface IAuthService
    {
        Task<Result<LoginCommandResponse>> Login(LoginCommand command);
        Task<Result<LoginCommandResponse>> ValidateRefreshToken(string? refreshToken);
        Task Logout();
    }
}
