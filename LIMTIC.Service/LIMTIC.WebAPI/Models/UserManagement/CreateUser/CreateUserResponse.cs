using LIMTIC.Application.DTOs.UserManagement;

namespace LIMTIC.WebAPI.Models.UserManagement.CreateUser
{
    public class CreateUserResponse : BaseResponse
    {
        public UserDto? User { get; set; }
    }
}
