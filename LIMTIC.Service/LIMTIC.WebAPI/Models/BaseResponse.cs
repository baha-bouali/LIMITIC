using LIMTIC.WebAPI.Models;

namespace LIMTIC.WebAPI
{
    public class BaseResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, IEnumerable<string>>? ValidationErrors { get; set; }
        public PaginationResponse? Pagination { get; set; }
    }
}
