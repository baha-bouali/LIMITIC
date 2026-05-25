using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class MasterianRepository : IMasterianRepository
    {
        private readonly AppDbContext _dbContext;
        public MasterianRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task<MasterianEntity?> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.Masterians
                .Include(m => m.User)
                .Include(m => m.Supervisor)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == userId);
        }

        public async Task<List<MasterianEntity>> GetAllAsync()
        {
            return await _dbContext.Masterians
                .Include(m => m.User)
                .Include(m => m.Supervisor)
                    .ThenInclude(s => s.User)
                .ToListAsync();
        }

        public async Task<bool> AddAsync(MasterianEntity entity)
        {
            _dbContext.Masterians.Add(entity);
            return (await _dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> UpdateAsync(MasterianEntity entity)
        {
            _dbContext.Masterians.Update(entity);
            return (await _dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> DeleteAsync(MasterianEntity entity)
        {
            _dbContext.Masterians.Remove(entity);
            return (await _dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> ExistsAsync(Guid userId)
        {
            return await _dbContext.Masterians.AnyAsync(m => m.Id == userId);
        }
    }
}
