using LIMTIC.WebAPI.Base;

namespace LIMTIC.Application.DTOs.UserManagement.CreateUser
{
    public class CreateUserResponse : BaseResponse
    {
        public UserDto? User { get; set; }
    }
}
