namespace LIMTIC.Application.Contracts.Commands.Publications
{
    public class GetPublicPublicationsCommand
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? Search { get; set; }
        public string? Type { get; set; }
        public int? Year { get; set; }
        public Guid? AxeId { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}

