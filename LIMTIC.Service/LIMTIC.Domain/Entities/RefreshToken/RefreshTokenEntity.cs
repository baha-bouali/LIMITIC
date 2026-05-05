using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.RefreshToken
{
    public class RefreshTokenEntity : BaseEntity
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }

        public Guid UserId { get; set; }

        public UserEntity? User { get; set; }
    }
}
