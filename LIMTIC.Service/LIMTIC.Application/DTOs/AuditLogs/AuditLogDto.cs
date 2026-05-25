namespace LIMTIC.Application.DTOs.AuditLogs
{
    public class AuditLogDto
    {
        public string Id { get; set; }
        public string ActorId { get; set; }
        public string Action { get; set; }
        public string Resource { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
