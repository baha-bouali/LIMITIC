using LIMTIC.Domain.Entities.Publications;

namespace LIMTIC.Domain.Abstractions
{
    public interface IJournalArticleRepository
    {
        Task<JournalArticleEntity?> GetByPublicationIdAsync(Guid publicationId, CancellationToken ct = default);
        Task AddAsync(JournalArticleEntity entity, CancellationToken ct = default);
        void Update(JournalArticleEntity entity);
        void Remove(JournalArticleEntity entity);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
