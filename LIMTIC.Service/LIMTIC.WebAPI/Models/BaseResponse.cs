namespace LIMTIC.WebAPI.Models
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, IEnumerable<string>>? ValidationErrors { get; set; }
    }
}
