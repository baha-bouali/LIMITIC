using LIMTIC.WebAPI.Base;

namespace LIMTIC.WebAPI.Models.Auth.Login
{
    public class LoginResponse : BaseResponse
    {
        public string? AccessToken { get; set; }
    }
}
