namespace LIMTIC.Application.DTOs.Storage
{
    public class FileDownloadDto
    {
        public Guid? FileId { get; set; }
        public required Stream Stream { get; set; }
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
    }
}
