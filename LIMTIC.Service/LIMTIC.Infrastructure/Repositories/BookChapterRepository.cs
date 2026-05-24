using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class BookChapterRepository : IBookChapterRepository
    {
        private readonly AppDbContext _context;

        public BookChapterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BookChapterEntity?> GetByPublicationIdAsync(Guid publicationId, CancellationToken ct = default)
        {
            // Note: because the Id of the specific entities is mapped as a ForeignKey to the parent PublicationId 
            // in your EF configuration, looking up by Id == publicationId is correct.
            return await _context.BookChapters
                .FirstOrDefaultAsync(b => b.Id == publicationId, ct);
        }

        public async Task AddAsync(BookChapterEntity entity, CancellationToken ct = default)
        {
            await _context.BookChapters.AddAsync(entity, ct);
        }

        public void Update(BookChapterEntity entity)
        {
            _context.BookChapters.Update(entity);
        }

        public void Remove(BookChapterEntity entity)
        {
            _context.BookChapters.Remove(entity);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}