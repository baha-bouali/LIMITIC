using LIMTIC.Domain.Entities.Users;

namespace LIMTIC.Application.Contracts.Commands.CreateUser
{
    public class CreateUserCommandResponse
    {
        public UserEntity User { get; set; }
    }
}
