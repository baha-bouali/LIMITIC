using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Logs;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
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
            if (_dbContext.Database.IsRelational())
            {
                await _dbContext.Database.ExecuteSqlRawAsync(
                    """
                    CREATE TABLE IF NOT EXISTS "AuditLogs" (
                        "Id" uuid NOT NULL,
                        "CreatedBy" uuid NOT NULL,
                        "CreatedAtUtc" timestamp with time zone NOT NULL,
                        "ActorId" uuid NOT NULL,
                        "Action" text NOT NULL,
                        "Resource" text NOT NULL,
                        "Timestamp" timestamp with time zone NOT NULL,
                        CONSTRAINT "PK_AuditLogs" PRIMARY KEY ("Id")
                    );

                    ALTER TABLE "AuditLogs"
                    ADD COLUMN IF NOT EXISTS "CreatedBy" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

                    ALTER TABLE "AuditLogs"
                    ADD COLUMN IF NOT EXISTS "CreatedAtUtc" timestamp with time zone NOT NULL DEFAULT NOW();

                    ALTER TABLE "AuditLogs"
                    ADD COLUMN IF NOT EXISTS "ActorId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
                    """);
            }

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
                .Where(l => l.Timestamp >= fromUtc && l.Timestamp <= toUtc)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }
    }
}
