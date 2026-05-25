namespace LIMTIC.Application.Contracts.Commands.Publications
{
    public class RejectDashboardPublicationCommand
    {
        public Guid Id { get; set; }
        public string? Reason { get; set; }
    }
}

