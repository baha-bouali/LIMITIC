namespace LIMTIC.Application.Contracts.Commands.Publications
{
    public class GetDashboardPublicationsCommand
    {
        public string Scope { get; set; } = "mine";
        public string? Search { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
        public string? Visibility { get; set; }
        public int? Year { get; set; }
        public Guid? AxeId { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }
}

