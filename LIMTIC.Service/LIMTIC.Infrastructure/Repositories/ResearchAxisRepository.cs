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

        public async Task<List<ResearchAxisEntity>> GetByIdsAsync(List<Guid> ids)
        {
            return await _dbContext.ResearchAxes
                .Where(a => ids.Contains(a.Id))
                .ToListAsync();
        }
    }
}
