using LIMTIC.Application.DTOs.UserManagement;

namespace LIMTIC.Application.Contracts.Commands.GetUser
{
    public class GetUserCommandResponse
    {
        public UserDto User { get; set; }
    }
}
