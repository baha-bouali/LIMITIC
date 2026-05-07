using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.ResetPassword;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class ResetPasswordRepository : IResetPasswordRepository
    {
        private readonly AppDbContext _dbContext;

        public ResetPasswordRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddResetPasswordAsync(ResetPasswordEntity resetPassword)
        {
            await _dbContext
                .ResetPasswords
                .AddAsync(resetPassword);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<ResetPasswordEntity?> GetResetPasswordAsync(Guid userId)
        {
            return await _dbContext
                .ResetPasswords
                .FirstOrDefaultAsync(rp => rp.UserId == userId);
        }

        public async Task<ResetPasswordEntity> UpdateResetPasswordAsync(ResetPasswordEntity resetPassword)
        {
            _dbContext.ResetPasswords.Update(resetPassword);
            await _dbContext.SaveChangesAsync();
            return resetPassword;
        }

        public async Task DeleteResetPasswordAsync(Guid userId)
        {
            await _dbContext
               .ResetPasswords
               .Where(e => e.UserId == userId)
               .ExecuteDeleteAsync();
        }
    }
}