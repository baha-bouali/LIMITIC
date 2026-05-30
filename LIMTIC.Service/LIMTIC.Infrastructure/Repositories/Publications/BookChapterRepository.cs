using LIMTIC.Domain.Abstractions.Publications;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories.Publications
{
    public class BookChapterRepository : IBookChapterRepository
    {
        private readonly AppDbContext _context;

        public BookChapterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BookChapterEntity?> GetByPublicationIdAsync(Guid publicationId)
        {
            return await _context.BookChapters
                .FirstOrDefaultAsync(b => b.Id == publicationId);
        }

        public async Task AddAsync(BookChapterEntity entity)
        {
            await _context.BookChapters.AddAsync(entity);
        }

        public void Update(BookChapterEntity entity)
        {
            _context.BookChapters.Update(entity);
        }

        public void Remove(BookChapterEntity entity)
        {
            _context.BookChapters.Remove(entity);
        }
    }
}