using LIMTIC.Domain.Entities;

namespace LIMTIC.Application.Abstractions.Security
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
