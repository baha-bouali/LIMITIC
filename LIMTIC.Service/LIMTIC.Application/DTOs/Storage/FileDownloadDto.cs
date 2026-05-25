using System.IO;

namespace LIMTIC.Application.DTOs.Storage
{
    public class FileDownloadDto
    {
        public required Stream Stream { get; set; }
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
    }
}
