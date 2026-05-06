using LIMTIC.Domain.Entities.Users;

namespace LIMTIC.Application.Contracts.Commands.GetUser
{
    public class GetUserCommandResponse
    {
        public UserEntity User { get; set; }
    }
}
