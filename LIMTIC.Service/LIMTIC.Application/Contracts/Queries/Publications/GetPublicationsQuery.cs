namespace LIMTIC.Application.Contracts.Queries.Publications
{
    public class GetPublicationsQuery
    {
        public string Scope { get; set; } = "mine";
        public string? Search { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
        public string? Visibility { get; set; }
        public Guid? UserId { get; set; }
        public int? Year { get; set; }
        public Guid? AxeId { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }
}

