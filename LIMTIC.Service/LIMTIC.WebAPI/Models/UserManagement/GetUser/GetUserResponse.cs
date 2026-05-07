
using LIMTIC.WebAPI;

namespace LIMTIC.Application.DTOs.UserManagement.GetUser
{
    public class GetUserResponse : BaseResponse
    {
        public UserDto User { get; set; }
    }
}
