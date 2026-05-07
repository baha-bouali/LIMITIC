using LIMTIC.Domain.Entities.Users;

namespace LIMTIC.Application.Abstractions.Security
{
    public interface ITokenService
    {
        string GenerateAccessToken(UserEntity user);
        string GenerateToken();
        String GenerateOTPToken();
    }
}
