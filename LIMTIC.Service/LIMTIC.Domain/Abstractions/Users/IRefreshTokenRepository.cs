using LIMTIC.Domain.Entities.RefreshToken;

namespace LIMTIC.Domain.Abstractions.Users
{
    public interface IRefreshTokenRepository
    {
        Task<int> AddRefreshTokenAsync(RefreshTokenEntity refreshToken);
        Task<RefreshTokenEntity?> GetRefreshTokenAsync(string token);
        Task<int> RevokeRefreshTokenAsync(string? token);
    }
}
