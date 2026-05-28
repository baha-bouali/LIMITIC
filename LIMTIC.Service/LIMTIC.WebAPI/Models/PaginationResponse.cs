namespace LIMTIC.WebAPI.Models
{
    public class PaginationResponse
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)Total / Limit);
    }
}

