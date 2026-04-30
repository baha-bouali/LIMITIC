namespace LIMTIC.Application.DTOs.Auth
{
    public record LoginResponse(
        string AccessToken,
        string RefreshToken)
    {
    }
}
