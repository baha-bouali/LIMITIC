namespace LIMTIC.Application.DTOs.Publications
{
    public sealed class BookChapterDto
    {
        public string BookTitle { get; set; } = string.Empty;

        public string Publisher { get; set; } = string.Empty;

        public string? Isbn { get; set; }

        public string? Pages { get; set; }
    }
}
