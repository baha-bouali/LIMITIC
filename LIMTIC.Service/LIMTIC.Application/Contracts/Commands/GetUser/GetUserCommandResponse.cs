using LIMTIC.Domain.Entities;

namespace LIMTIC.Application.Contracts.Commands.GetUser
{
    public class GetUserCommandResponse
    {
        public User User { get; set; }
    }
}
