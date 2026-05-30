using LIMTIC.Application.Abstractions.AuditLogs;
using LIMTIC.Application.Contracts.Queries.AuditLogs;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.AuditLogs;
using LIMTIC.Domain.Abstractions.AuditLogs;

namespace LIMTIC.Application.Services.AuditLogs
{
    public class AuditLogsService : IAuditLogsService
    {
        private readonly IAuditLogsRepository _auditLogsRepository;

        public AuditLogsService(IAuditLogsRepository auditLogsRepository)
        {
            _auditLogsRepository = auditLogsRepository;
        }

        public async Task<Result<List<AuditLogDto>>> GetAuditLogsByPeriodAsync(GetAuditLogsQuery getAuditLogsQuery)
        {
            if (getAuditLogsQuery.FromUtc == default)
                return Result<List<AuditLogDto>>.FailureResult("fromUtc is required");

            // Npgsql requires DateTimeKind.Utc for timestamp with time zone columns.
            // Model binding from query string produces DateTimeKind.Unspecified, so we normalize here.
            var from = DateTime.SpecifyKind(getAuditLogsQuery.FromUtc, DateTimeKind.Utc);
            var rightBound = getAuditLogsQuery.ToUtc.HasValue
                ? DateTime.SpecifyKind(getAuditLogsQuery.ToUtc.Value, DateTimeKind.Utc)
                : DateTime.UtcNow;

            if (from > rightBound)
                return Result<List<AuditLogDto>>.FailureResult("fromUtc must be less than or equal to toUtc");

            var logs = await _auditLogsRepository.GetLogsByPeriodAsync(from, rightBound);
            var mapped = logs.Select(l => new AuditLogDto
            {
                Id = l.Id.ToString(),
                ActorId = l.ActorId.ToString(),
                ActorName = l.Actor is { } a ? $"{a.FirstName} {a.LastName}".Trim() : null,
                Action = l.Action.ToString(),
                Resource = l.Resource.ToString(),
                Timestamp = l.Timestamp
            }).ToList();

            return Result<List<AuditLogDto>>.SuccessResult(mapped);
        }
    }
}
