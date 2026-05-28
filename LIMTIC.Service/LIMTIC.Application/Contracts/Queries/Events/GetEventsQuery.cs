namespace LIMTIC.Application.Contracts.Queries.Events
{
    public class GetEventsQuery
    {
        public string? Status { get; set; }

        public string? Type { get; set; }

        public int Page { get; set; } = 1;

        public int Limit { get; set; } = 20;

        public string? Q { get; set; }
    }
}
