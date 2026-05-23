using LIMTIC.Domain.Entities.Publications;

namespace LIMTIC.Domain.Abstractions
{
    public interface IBookChapterRepository
    {
        Task<BookChapterEntity?> GetByPublicationIdAsync(Guid publicationId, CancellationToken ct = default);
        Task AddAsync(BookChapterEntity entity, CancellationToken ct = default);
        void Update(BookChapterEntity entity);
        void Remove(BookChapterEntity entity);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
