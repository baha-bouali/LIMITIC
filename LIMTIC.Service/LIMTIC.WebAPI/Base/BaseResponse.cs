namespace LIMTIC.WebAPI.Base
{
    public class BaseResponse
    {
        public string? Message { get; set; }
        public Dictionary<string, IEnumerable<string>>? ValidationErrors { get; set; }
    }
}
