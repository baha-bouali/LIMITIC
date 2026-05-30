namespace LIMTIC.Application.Contracts.Queries.AuditLogs
{
    public class GetAuditLogsQuery
    {
        public DateTime FromUtc { get; set; }
        public DateTime? ToUtc { get; set; } = null;
    }
}
