namespace LIMTIC.Application.Contracts.Commands.Login
{
    public record LoginCommandResponse(
        string AccessToken,
        string RefreshToken,
        Guid UserId,
        string Email)
    {
    }
}
