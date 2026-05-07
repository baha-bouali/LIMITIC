using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.RefreshToken;
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

        public async Task AddRefreshTokenAsync(RefreshTokenEntity refreshToken)
        {
            await _dbContext
                .RefreshTokens
                .AddAsync(refreshToken);
        }

        public async Task<RefreshTokenEntity?> GetRefreshTokenAsync(string token)
        {
            return await _dbContext
                .RefreshTokens
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Token == token);
        }

        public async Task RevokeRefreshTokenAsync(Guid userId)
        {
            await _dbContext
                .RefreshTokens
                .Where(e => e.UserId == userId)
                .ExecuteDeleteAsync();
        }
    }
}
