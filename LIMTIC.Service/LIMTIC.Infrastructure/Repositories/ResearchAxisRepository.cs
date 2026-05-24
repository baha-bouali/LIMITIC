using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.ResearchAxis;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class ResearchAxisRepository : IResearchAxisRepository
    {
        private readonly AppDbContext _dbContext;
        public ResearchAxisRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task<List<ResearchAxisEntity>> GetAllAsync()
            => await _dbContext.ResearchAxes.ToListAsync();

        public async Task<ResearchAxisEntity?> GetByIdAsync(Guid id)
            => await _dbContext.ResearchAxes.FirstOrDefaultAsync(a => a.Id == id);

        public async Task<List<ResearchAxisEntity>> GetByIdsAsync(List<Guid> ids)
            => await _dbContext.ResearchAxes.Where(a => ids.Contains(a.Id)).ToListAsync();

        public async Task<bool> AddAsync(ResearchAxisEntity entity)
        {
            _dbContext.ResearchAxes.Add(entity);
            return (await _dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> UpdateAsync(ResearchAxisEntity entity)
        {
            _dbContext.ResearchAxes.Update(entity);
            return (await _dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> DeleteAsync(ResearchAxisEntity entity)
        {
            _dbContext.ResearchAxes.Remove(entity);
            return (await _dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> ExistsAsync(Guid id)
            => await _dbContext.ResearchAxes.AnyAsync(a => a.Id == id);
    }
}
