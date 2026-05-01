namespace LIMTIC.Application.Commands.Login
{
    public record LoginCommandResponse(
        string AccessToken,
        string RefreshToken)
    {
    }
}
