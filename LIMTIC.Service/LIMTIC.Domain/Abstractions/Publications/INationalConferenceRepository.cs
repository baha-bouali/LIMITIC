using LIMTIC.Domain.Entities.Publications;

namespace LIMTIC.Domain.Abstractions.Publications
{
    public interface INationalConferenceRepository
    {
        Task<NationalConferenceEntity?> GetByPublicationIdAsync(Guid publicationId);
        Task AddAsync(NationalConferenceEntity entity);
        void Update(NationalConferenceEntity entity);
        void Remove(NationalConferenceEntity entity);
    }
}
