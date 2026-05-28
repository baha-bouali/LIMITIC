using LIMTIC.Domain.Entities.Files;

namespace LIMTIC.Domain.Abstractions.Files
{
    public interface IPublicationFilesRepository
    {
        Task AddAsync(PublicationFileEntity file);

        Task AddRangeAsync(IEnumerable<PublicationFileEntity> files);

        Task<PublicationFileEntity?> GetByIdAsync(Guid id);

        Task<IReadOnlyList<PublicationFileEntity>> GetByPublicationIdAsync(Guid publicationId);

        Task DeleteAsync(PublicationFileEntity file);

        Task DeleteByIdAsync(Guid id);

        Task DeleteByPublicationIdAsync(Guid publicationId);
    }
}
