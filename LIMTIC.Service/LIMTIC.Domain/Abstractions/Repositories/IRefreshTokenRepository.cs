using LIMTIC.Domain.Entities;

namespace LIMTIC.Application.Abstractions.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task RevokeRefreshToken(Guid userId);
    }
}
