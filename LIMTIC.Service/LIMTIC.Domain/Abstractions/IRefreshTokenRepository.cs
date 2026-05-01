using LIMTIC.Domain.Entities;

namespace LIMTIC.Domain.Abstractions
{
    public interface IRefreshTokenRepository
    {
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task RevokeRefreshToken(Guid userId);
    }
}
