
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Logs
{
    public class AuditLogsEntity : BaseEntity
    {
        public Guid ActorId { get; set; }
        public ActionType Action { get; set; }
        public ResourceType Resource { get; set; }
        public DateTime Timestamp { get; set; }

        public UserEntity? Actor { get; set; }
    }
}
