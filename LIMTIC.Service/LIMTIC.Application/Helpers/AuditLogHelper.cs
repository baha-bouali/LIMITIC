using LIMTIC.Domain.Entities.Logs;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Helpers
{
    public static class AuditLogHelper
    {
        public static AuditLogsEntity CreateAuditLog(Guid actorId, ActionType actionType, ResourceType resourceType)
        {
            return new AuditLogsEntity
            {
                Id = Guid.NewGuid(),
                ActorId = actorId,
                Action = actionType,
                Resource = resourceType,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
