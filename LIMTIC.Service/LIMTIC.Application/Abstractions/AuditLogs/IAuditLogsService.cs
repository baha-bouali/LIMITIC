using LIMTIC.Application.Contracts.Queries.AuditLogs;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.AuditLogs;

namespace LIMTIC.Application.Abstractions.AuditLogs
{
    public interface IAuditLogsService
    {
        Task<Result<List<AuditLogDto>>> GetAuditLogsByPeriodAsync(GetAuditLogsQuery getAuditLogsQuery);
    }
}
