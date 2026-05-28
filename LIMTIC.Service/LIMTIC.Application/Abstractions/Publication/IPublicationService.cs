using LIMTIC.Application.Contracts.Commands.Publications;
using LIMTIC.Application.Contracts.Queries.Publications;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Publications;
using LIMTIC.Application.DTOs.Storage;

namespace LIMTIC.Application.Abstractions.Publication
{
    public interface IPublicationService
    {
        Task<Result<(List<PublicationDto> Publications, int Total)>> GetPublicationsAsync(GetPublicationsQuery getPublicationsQuery);
        Task<Result<PublicationDto>> GetPublicationByIdAsync(Guid publicationId);
        Task<Result<List<FileDownloadDto>>> GetPublicationPdfsAsync(Guid publicationId);
        Task<Result<PublicationDto>> CreatePublicationAsync(CreatePublicationCommand command);
        Task<Result<bool>> UpdatePublicationAsync(UpdatePublicationCommand command);
        Task<Result<bool>> DeletePublicationAsync(Guid publicationId);
        Task<Result<bool>> SubmitPublicationAsync(Guid publicationId);
        Task<Result<bool>> ValidatePublicationAsync(Guid publicationId);
        Task<Result<bool>> RejectPublicationAsync(Guid publicationId);
        Task<Result<List<FileDownloadDto>>> GetPublicationPdfs(Guid publicationId); 
        Task<Result<bool>> AddPublicationPdfsAsync(AddPublicationPdfsCommand command);
        Task<Result<bool>> RemovePublicationPdfAsync(RemovePublicationPdfCommand command);
    }
}
