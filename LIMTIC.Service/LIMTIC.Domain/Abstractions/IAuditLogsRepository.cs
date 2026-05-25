using LIMTIC.Domain.Entities.Logs;

namespace LIMTIC.Domain.Abstractions
{
    public interface IAuditLogsRepository
    {
        public Task<bool> AddLog(AuditLogsEntity log);
        public Task<bool> DeleteLog(Guid logId);
        public Task<List<AuditLogsEntity>> GetLogsByPeriodAsync(DateTime fromUtc, DateTime toUtc);
    }
}
