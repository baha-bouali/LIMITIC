using LIMTIC.Domain.Entities.Publications;

namespace LIMTIC.Domain.Abstractions.Publications
{
    public interface IBookChapterRepository
    {
        Task<BookChapterEntity?> GetByPublicationIdAsync(Guid publicationId);
        Task AddAsync(BookChapterEntity entity);
        void Update(BookChapterEntity entity);
        void Remove(BookChapterEntity entity);
    }
}
