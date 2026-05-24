namespace LIMTIC.Application.Contracts.Commands.CreateUser
{
    public class CreateUserCommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
    }
}
