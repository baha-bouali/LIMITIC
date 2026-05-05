using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _dbContext;

        public RefreshTokenRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _dbContext
                .RefreshTokens
                .AddAsync(refreshToken);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _dbContext
                .RefreshTokens
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Token == token);
        }

        public async Task<int> RevokeRefreshTokenAsync(string? token)
        {
            return await _dbContext
                .RefreshTokens
                .Where(e => e.Token == token)
                .ExecuteDeleteAsync();
        }
    }
}
