using LIMTIC.Application.DTOs.AuditLogs;
using LIMTIC.WebAPI;

namespace LIMTIC.WebAPI.Models.AuditLogs.GetAuditLogs
{
    public class GetAuditLogsResponse : BaseResponse
    {
        public List<AuditLogDto> Items { get; set; } = [];
        public DateTime FromUtc { get; set; }
        public DateTime ToUtc { get; set; }
    }
}
