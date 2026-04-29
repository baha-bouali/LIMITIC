using LIMTIC.Domain.Enums;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? AvatarBlobName { get; set; }
        public bool IsActive { get; set; }
    }
}
