using LIMTIC.Domain.Enums;

namespace LIMTIC.WebAPI.Models.UserManagement.CreateUser
{
    public class CreateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public UserRole Role { get; set; }
    }
}
