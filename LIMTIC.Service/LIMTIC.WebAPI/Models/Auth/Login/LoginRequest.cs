namespace LIMTIC.WebAPI.Models.Auth.Login
{
    public record LoginRequest(
        string Username,
        string Password)
    {
    }
}
