using LIMTIC.Domain.Enums;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Users
{
    public class UserEntity : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? AvatarBlobName { get; set; }
        public bool IsActive { get; set; }

        public ResearcherEntity? Researcher { get; set; }
        public PhDStudentEntity? PhDStudent { get; set; }
        public MasterianEntity? Masterian { get; set; }

        public static UserEntity Create(string email, string firstName, string lastName, string passwordHash, UserRole role, bool isActive, string? avatarBlobName = null)
        {
            var user = new UserEntity
            {
                Email = email.ToLower(),
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                PasswordHash = passwordHash,
                Role = role,
                IsActive = isActive,
                AvatarBlobName = avatarBlobName
            };

            return user;
        }
    }
}
