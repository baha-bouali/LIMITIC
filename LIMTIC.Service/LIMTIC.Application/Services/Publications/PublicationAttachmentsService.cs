using LIMTIC.Application.Abstractions.Publications;
using LIMTIC.Application.Abstractions.Storage;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Storage;
using LIMTIC.Domain.Abstractions;
using System.IO;

namespace LIMTIC.Application.Services.Publications
{
    public class PublicationAttachmentsService : IPublicationAttachmentsService
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly IBlobStorageService _blobStorageService;

        public PublicationAttachmentsService(IPublicationRepository publicationRepository, IBlobStorageService blobStorageService)
        {
            _publicationRepository = publicationRepository;
            _blobStorageService = blobStorageService;
        }

        public async Task<Result<bool>> UploadAttachmentsAsync(Guid publicationId, List<(Stream Stream, string FileName)> files)
        {
            if (files == null || files.Count == 0)
                return Result<bool>.FailureResult("No files provided");

            var publication = await _publicationRepository.GetByIdAsync(publicationId);
            if (publication == null)
                return Result<bool>.FailureResult("Publication not found");

            var attachedPdfs = publication.AttachedPdfs?.ToList() ?? new List<string>();
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext != ".pdf")
                    return Result<bool>.FailureResult($"Only PDF files are supported: {file.FileName}");

                var blobName = $"publications/{publicationId}/attachments/{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var uploaded = await _blobStorageService.UploadStreamAsync(file.Stream, "media", blobName, overwrite: true);
                if (!uploaded)
                    return Result<bool>.FailureResult($"Failed to upload PDF: {file.FileName}");

                attachedPdfs.Add(blobName);
            }

            publication.AttachedPdfs = attachedPdfs.ToArray();
            _publicationRepository.Update(publication);
            return Result<bool>.SuccessResult(true);
        }

        public async Task<Result<FileDownloadDto>> GetAttachmentAsync(Guid publicationId, int index)
        {
            var publication = await _publicationRepository.GetByIdAsync(publicationId);
            if (publication == null || publication.AttachedPdfs == null || index < 0 || index >= publication.AttachedPdfs.Length)
                return Result<FileDownloadDto>.FailureResult("Attachment not found");

            var blobName = publication.AttachedPdfs[index];
            try
            {
                var stream = await _blobStorageService.GetStreamAsync("media", blobName);
                return Result<FileDownloadDto>.SuccessResult(new FileDownloadDto
                {
                    Stream = stream,
                    FileName = Path.GetFileName(blobName),
                    ContentType = "application/pdf"
                });
            }
            catch (FileNotFoundException)
            {
                return Result<FileDownloadDto>.FailureResult("Attachment file not found in blob storage");
            }
        }
    }
}
