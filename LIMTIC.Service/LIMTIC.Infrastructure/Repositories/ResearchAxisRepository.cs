using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.ResearchAxis;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class ResearchAxisRepository : IResearchAxisRepository
    {
        private readonly AppDbContext _dbContext;
        public ResearchAxisRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task<List<ResearchAxisEntity>> GetAllAsync()
            => await _dbContext.ResearchAxes
                .Include(a => a.Responsible).ThenInclude(r => r!.User)
                .Include(a => a.Researchers).ThenInclude(r => r.User)
                .Include(a => a.Publications)
                .ToListAsync();

        public async Task<ResearchAxisEntity?> GetByIdAsync(Guid id)
            => await _dbContext.ResearchAxes
                .Include(a => a.Responsible).ThenInclude(r => r!.User)
                .Include(a => a.Researchers).ThenInclude(r => r.User)
                .Include(a => a.Publications)
                .FirstOrDefaultAsync(a => a.Id == id);

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

        public async Task<bool> AddMemberAsync(Guid axisId, Guid researcherUserId)
        {
            var axis = await _dbContext.ResearchAxes
                .Include(a => a.Researchers)
                .FirstOrDefaultAsync(a => a.Id == axisId);

            if (axis is null) return false;

            if (axis.Researchers.Any(r => r.Id == researcherUserId))
                return true;

            var researcher = await _dbContext.Researchers.FindAsync(researcherUserId);
            if (researcher is null) return false;

            axis.Researchers.Add(researcher);
            return (await _dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> RemoveMemberAsync(Guid axisId, Guid researcherUserId)
        {
            var axis = await _dbContext.ResearchAxes
                .Include(a => a.Researchers)
                .FirstOrDefaultAsync(a => a.Id == axisId);

            if (axis is null) return false;

            var researcher = axis.Researchers.FirstOrDefault(r => r.Id == researcherUserId);
            if (researcher is null) return false;

            axis.Researchers.Remove(researcher);
            return (await _dbContext.SaveChangesAsync()) > 0;
        }
    }
}
