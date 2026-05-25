namespace LIMTIC.Application.Contracts.Commands.Publications
{
    public class GetPublicPublicationByIdCommand
    {
        public Guid Id { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}

