namespace LIMTIC.Application.Contracts.Commands.Publications
{
    public class GetRecentPublicPublicationsCommand
    {
        public int Limit { get; set; } = 3;
    }
}

