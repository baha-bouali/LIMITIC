using LIMTIC.Domain.Entities.Users;

namespace LIMTIC.Domain.Entities.ResetPassword
{
    public class ResetPasswordEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string OTPTokenHash { get; set; } = string.Empty;
        public DateTime OTPTokenExpiry { get; set; }
        public string? ResetPasswordTokenHash { get; set; }
        public DateTime? ResetPasswordTokenExpiry { get; set; }
        public UserEntity? User { get; set; }
    }
}
