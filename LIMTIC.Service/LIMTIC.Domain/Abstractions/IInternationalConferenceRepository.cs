using LIMTIC.Domain.Entities.Publications;

namespace LIMTIC.Domain.Abstractions
{
    public interface IInternationalConferenceRepository
    {
        Task<InternationalConferenceEntity?> GetByPublicationIdAsync(Guid publicationId, CancellationToken ct = default);
        Task AddAsync(InternationalConferenceEntity entity, CancellationToken ct = default);
        void Update(InternationalConferenceEntity entity);
        void Remove(InternationalConferenceEntity entity);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
