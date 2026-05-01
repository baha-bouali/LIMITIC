using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Commands.CreateUser
{
    public class CreateUserCommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
    }
}
