using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class JournalArticleRepository : IJournalArticleRepository
    {
        private readonly AppDbContext _context;

        public JournalArticleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<JournalArticleEntity?> GetByPublicationIdAsync(Guid publicationId, CancellationToken ct = default)
        {
            return await _context.JournalArticles
                .FirstOrDefaultAsync(ja => ja.Id == publicationId, ct);
        }

        public async Task AddAsync(JournalArticleEntity entity, CancellationToken ct = default)
        {
            await _context.JournalArticles.AddAsync(entity, ct);
        }

        public void Update(JournalArticleEntity entity)
        {
            _context.JournalArticles.Update(entity);
        }

        public void Remove(JournalArticleEntity entity)
        {
            _context.JournalArticles.Remove(entity);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}