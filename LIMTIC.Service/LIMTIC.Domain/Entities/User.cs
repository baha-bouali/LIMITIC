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
       
        public static Result<User> Create(string email, string firstName, string lastName, string passwordHash, UserRole role, bool isActive, string? avatarBlobName = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result<User>.FailureResult("Email is required");

            if (!email.Contains("@"))
                return Result<User>.FailureResult("Email format invalid");

            if (string.IsNullOrWhiteSpace(firstName))
                return Result<User>.FailureResult("First name is required");

            if (string.IsNullOrWhiteSpace(lastName))
                return Result<User>.FailureResult("Last name is required");
            
            if (string.IsNullOrWhiteSpace(passwordHash))
                return Result<User>.FailureResult("Password is required");

            var user = new User
            {
                Email = email.ToLower(),
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                PasswordHash = passwordHash,
                Role = role,
                IsActive = isActive,
                AvatarBlobName = avatarBlobName
            };

            return Result<User>.SuccessResult(user);
        }
    }
}
