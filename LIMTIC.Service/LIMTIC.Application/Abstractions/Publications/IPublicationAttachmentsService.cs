using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Storage;

namespace LIMTIC.Application.Abstractions.Publications
{
    public interface IPublicationAttachmentsService
    {
        Task<Result<bool>> UploadAttachmentsAsync(Guid publicationId, List<(Stream Stream, string FileName)> files);
        Task<Result<FileDownloadDto>> GetAttachmentAsync(Guid publicationId, int index);
    }
}
