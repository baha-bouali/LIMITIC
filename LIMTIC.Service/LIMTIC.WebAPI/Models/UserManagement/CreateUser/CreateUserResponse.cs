using LIMTIC.WebAPI.Base;

namespace LIMTIC.WebAPI.Models.UserManagement.CreateUser
{
    public class CreateUserResponse : BaseResponse
    {
        public UserDto? User { get; set; }
    }
}
