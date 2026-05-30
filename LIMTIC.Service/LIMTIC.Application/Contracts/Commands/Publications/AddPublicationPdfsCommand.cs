using LIMTIC.Application.DTOs.Storage;

namespace LIMTIC.Application.Contracts.Commands.Publications
{
    public class AddPublicationPdfsCommand
    {
        public Guid PublicationId { get; set; }
        public List<FileDownloadDto> FileDownloads { get; set; }
    }
}
