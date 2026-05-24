using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class ResearcherRepository : IResearcherRepository
    {
        private readonly AppDbContext _dbContext;
        public ResearcherRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task<ResearcherEntity?> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.Researchers
                .Include(r => r.User)
                .Include(r => r.ResearchAxes)
                .FirstOrDefaultAsync(r => r.Id == userId);
        }

        public async Task<bool> AddAsync(ResearcherEntity entity)
        {
            _dbContext.Researchers.Add(entity);
            return (await _dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> UpdateAsync(ResearcherEntity entity)
        {
            _dbContext.Researchers.Update(entity);
            return (await _dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> DeleteAsync(ResearcherEntity entity)
        {
            _dbContext.Researchers.Remove(entity);
            return (await _dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> ExistsAsync(Guid userId)
        {
            return await _dbContext.Researchers.AnyAsync(r => r.Id == userId);
        }
    }
}
