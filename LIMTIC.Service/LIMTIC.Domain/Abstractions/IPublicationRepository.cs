using LIMTIC.Domain.Entities.Publications;

namespace LIMTIC.Domain.Abstractions
{
    public interface IPublicationRepository
    {
        Task<PublicationEntity?> GetByIdAsync(Guid publicationId);
        Task<bool> UpdateAsync(PublicationEntity publication);
    }
}
