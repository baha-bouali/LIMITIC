using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Entities.ResearchAxis;
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
            try
            {
                // Try to fetch researcher with research axes (normal path)
                return await _dbContext.Researchers
                    .Include(r => r.User)
                    .Include(r => r.ResearchAxes)
                    .FirstOrDefaultAsync(r => r.Id == userId);
            }
            catch (Exception)
            {
                // If the database schema is missing the join table (e.g., migrations not applied),
                // fall back to fetching researcher without research axes to avoid throwing 500.
                var researcher = await _dbContext.Researchers
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.Id == userId);
                return researcher;
            }
        }

        public async Task<List<ResearcherEntity>> GetAllAsync()
        {
            return await _dbContext.Researchers
                .Include(r => r.User)
                .Include(r => r.ResearchAxes)
                .ToListAsync();
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
