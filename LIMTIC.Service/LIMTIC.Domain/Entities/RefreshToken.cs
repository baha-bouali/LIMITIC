using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }

        public Guid UserId { get; set; }

        public User? User { get; set; }
    }
}
