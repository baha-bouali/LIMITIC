using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class PublicationsRepository : IPublicationsRepository
    {
        private readonly AppDbContext _context;
        public PublicationsRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PublicationEntity?> GetPublicationByIdAsync(Guid publicationId)
        {
            return await _context.Publications
                .FirstOrDefaultAsync(p => p.Id == publicationId);
        }

        public async Task<IReadOnlyList<PublicationEntity>> GetPublicationsByUserIdAsync(Guid userId)
        {
            return await _context.Publications
                .Where(p => p.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> AddPublicationAsync(PublicationEntity publication)
        {
            await _context.Publications.AddAsync(publication);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdatePublicationAsync(PublicationEntity publication)
        {
            _context.Publications.Update(publication);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeletePublicationAsync(Guid publicationId)
        {
            var result = await _context.Publications
                .Where(p => p.Id == publicationId)
                .ExecuteDeleteAsync();

            return result > 0;
        }
    }
}