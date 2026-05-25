using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.AuditLogs;

namespace LIMTIC.Application.Abstractions.AuditLogs
{
    public interface IAuditLogsService
    {
        Task<Result<List<AuditLogDto>>> GetAuditLogsByPeriodAsync(DateTime fromUtc, DateTime? toUtc);
    }
}
