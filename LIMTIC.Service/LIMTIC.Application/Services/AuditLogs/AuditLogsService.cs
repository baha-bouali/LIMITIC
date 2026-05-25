using LIMTIC.Application.Abstractions.AuditLogs;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.AuditLogs;
using LIMTIC.Domain.Abstractions;

namespace LIMTIC.Application.Services.AuditLogs
{
    public class AuditLogsService : IAuditLogsService
    {
        private readonly IAuditLogsRepository _auditLogsRepository;

        public AuditLogsService(IAuditLogsRepository auditLogsRepository)
        {
            _auditLogsRepository = auditLogsRepository;
        }

        public async Task<Result<List<AuditLogDto>>> GetAuditLogsByPeriodAsync(DateTime fromUtc, DateTime? toUtc)
        {
            if (fromUtc == default)
                return Result<List<AuditLogDto>>.FailureResult("fromUtc is required");

            var rightBound = toUtc ?? DateTime.UtcNow;
            if (fromUtc > rightBound)
                return Result<List<AuditLogDto>>.FailureResult("fromUtc must be less than or equal to toUtc");

            var logs = await _auditLogsRepository.GetLogsByPeriodAsync(fromUtc, rightBound);
            var mapped = logs.Select(l => new AuditLogDto
            {
                Id = l.Id.ToString(),
                ActorId = l.ActorId.ToString(),
                Action = l.Action.ToString(),
                Resource = l.Resource.ToString(),
                Timestamp = l.Timestamp
            }).ToList();

            return Result<List<AuditLogDto>>.SuccessResult(mapped);
        }
    }
}
