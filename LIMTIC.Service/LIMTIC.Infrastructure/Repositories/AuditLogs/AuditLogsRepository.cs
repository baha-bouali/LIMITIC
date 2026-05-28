using LIMTIC.Domain.Abstractions.AuditLogs;
using LIMTIC.Domain.Entities.Logs;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories.AuditLogs
{
    public class AuditLogsRepository : IAuditLogsRepository
    {
        private readonly AppDbContext _dbContext;
        
        public AuditLogsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddLog(AuditLogsEntity log)
        {
            await _dbContext.AuditLogs.AddAsync(log);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteLog(Guid logId)
        {
            return await _dbContext
               .AuditLogs
               .Where(e => e.Id == logId)
               .ExecuteDeleteAsync() > 0;
        }

        public async Task<List<AuditLogsEntity>> GetLogsByPeriodAsync(DateTime fromUtc, DateTime toUtc)
        {
            return await _dbContext.AuditLogs
                .Include(l => l.Actor)
                .Where(l => l.Timestamp >= fromUtc && l.Timestamp <= toUtc)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }
    }
}
