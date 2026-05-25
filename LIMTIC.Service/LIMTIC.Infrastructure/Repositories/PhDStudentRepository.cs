using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class PhDStudentRepository : IPhDStudentRepository
    {
        private readonly AppDbContext _dbContext;
        public PhDStudentRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task<PhDStudentEntity?> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.PhDStudents
                .Include(p => p.User)
                .Include(p => p.Supervisor)
                    .ThenInclude(s => s.User)
                .Include(p => p.ResearchAxes)
                .FirstOrDefaultAsync(p => p.Id == userId);
        }

        public async Task<List<PhDStudentEntity>> GetAllAsync()
        {
            return await _dbContext.PhDStudents
                .Include(p => p.User)
                .Include(p => p.Supervisor)
                    .ThenInclude(s => s.User)
                .Include(p => p.ResearchAxes)
                .ToListAsync();
        }

        public async Task<bool> AddAsync(PhDStudentEntity entity)
        {
            _dbContext.PhDStudents.Add(entity);
            return (await _dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> UpdateAsync(PhDStudentEntity entity)
        {
            _dbContext.PhDStudents.Update(entity);
            return (await _dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> DeleteAsync(PhDStudentEntity entity)
        {
            _dbContext.PhDStudents.Remove(entity);
            return (await _dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> ExistsAsync(Guid userId)
        {
            return await _dbContext.PhDStudents.AnyAsync(p => p.Id == userId);
        }
    }
}
