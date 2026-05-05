using LIMTIC.Domain.Entities;

namespace LIMTIC.Domain.Abstractions
{
    public interface IRefreshTokenRepository
    {
        Task<int> AddRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task<int> RevokeRefreshTokenAsync(Guid userId);
    }
}
