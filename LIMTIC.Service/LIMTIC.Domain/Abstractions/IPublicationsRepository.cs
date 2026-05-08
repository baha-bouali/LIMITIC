using LIMTIC.Domain.Entities.Publications;

namespace LIMTIC.Domain.Abstractions
{
    public interface IPublicationsRepository
    {
        Task<PublicationEntity?> GetPublicationByIdAsync(Guid publicationId);
        Task<IReadOnlyList<PublicationEntity>> GetPublicationsByUserIdAsync(Guid userId);
        Task<bool> AddPublicationAsync(PublicationEntity publication);
        Task<bool> UpdatePublicationAsync(PublicationEntity publication);
        Task<bool> DeletePublicationAsync(Guid publicationId);
    }
}
