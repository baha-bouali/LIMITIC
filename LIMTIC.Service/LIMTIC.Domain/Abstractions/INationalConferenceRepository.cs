using LIMTIC.Domain.Entities.Publications;

namespace LIMTIC.Domain.Abstractions
{
    public interface INationalConferenceRepository
    {
        Task<NationalConferenceEntity?> GetByPublicationIdAsync(Guid publicationId, CancellationToken ct = default);
        Task AddAsync(NationalConferenceEntity entity, CancellationToken ct = default);
        void Update(NationalConferenceEntity entity);
        void Remove(NationalConferenceEntity entity);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
