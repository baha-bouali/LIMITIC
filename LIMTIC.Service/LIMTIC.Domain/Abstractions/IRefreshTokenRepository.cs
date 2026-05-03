using LIMTIC.Domain.Entities.RefreshToken;

namespace LIMTIC.Domain.Abstractions
{
    public interface IRefreshTokenRepository
    {
        Task AddRefreshTokenAsync(RefreshTokenEntity refreshToken);
        Task<RefreshTokenEntity?> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(Guid userId);
    }
}
