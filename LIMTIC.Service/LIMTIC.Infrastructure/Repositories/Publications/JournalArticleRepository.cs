using LIMTIC.Domain.Abstractions.Publications;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories.Publications
{
    public class JournalArticleRepository : IJournalArticleRepository
    {
        private readonly AppDbContext _context;

        public JournalArticleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<JournalArticleEntity?> GetByPublicationIdAsync(Guid publicationId)
        {
            return await _context.JournalArticles
                .FirstOrDefaultAsync(ja => ja.Id == publicationId);
        }

        public async Task AddAsync(JournalArticleEntity entity)
        {
            await _context.JournalArticles.AddAsync(entity);
        }

        public void Update(JournalArticleEntity entity)
        {
            _context.JournalArticles.Update(entity);
        }

        public void Remove(JournalArticleEntity entity)
        {
            _context.JournalArticles.Remove(entity);
        }
    }
}