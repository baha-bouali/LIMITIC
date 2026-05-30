using LIMTIC.Domain.Entities.Publications;

namespace LIMTIC.Domain.Abstractions.Publications
{
    public interface IInternationalConferenceRepository
    {
        Task<InternationalConferenceEntity?> GetByPublicationIdAsync(Guid publicationId);
        Task AddAsync(InternationalConferenceEntity entity);
        void Update(InternationalConferenceEntity entity);
        void Remove(InternationalConferenceEntity entity);
    }
}
