namespace LIMTIC.WebAPI.Models.Auth.Login
{
    public class LoginResponse : BaseResponse
    {
        public string? AccessToken { get; set; }
        public Guid? UserId { get; set; }
        public string? Email { get; set; }
    }
}
