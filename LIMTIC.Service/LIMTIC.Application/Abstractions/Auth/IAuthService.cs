using LIMTIC.Application.Contracts.Commands.Login;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Application.Abstractions.Auth
{
    public interface IAuthService
    {
        Task<Result<LoginCommandResponse>> Login(LoginCommand command);
        Task<Result<LoginCommandResponse>> ValidateRefreshToken(string? refreshToken);
        Task Logout(string? refreshToken);
    }
}
