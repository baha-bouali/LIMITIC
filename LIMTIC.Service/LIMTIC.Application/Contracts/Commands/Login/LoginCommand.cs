namespace LIMTIC.Application.Contracts.Commands.Login
{
    public record LoginCommand(
        string Username,
        string Password)
    {
    }
}
