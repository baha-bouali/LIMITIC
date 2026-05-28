using LIMTIC.Domain.Entities.Publications;

namespace LIMTIC.Domain.Abstractions.Publications
{
    public interface IJournalArticleRepository
    {
        Task<JournalArticleEntity?> GetByPublicationIdAsync(Guid publicationId);
        Task AddAsync(JournalArticleEntity entity);
        void Update(JournalArticleEntity entity);
        void Remove(JournalArticleEntity entity);
    }
}
