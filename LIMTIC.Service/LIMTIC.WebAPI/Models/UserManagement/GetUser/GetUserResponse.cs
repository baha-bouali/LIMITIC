using LIMTIC.WebAPI.Base;

namespace LIMTIC.WebAPI.Models.UserManagement.GetUser
{
    public class GetUserResponse : BaseResponse
    {
        public UserDto User { get; set; }
    }
}
