using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class PublicationRepository : IPublicationRepository
    {
        private readonly AppDbContext _context;

        public PublicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<PublicationEntity?> GetByIdAsync(Guid publicationId)
        {
            return _context.Publications.FirstOrDefaultAsync(p => p.Id == publicationId);
        }

        public async Task<bool> UpdateAsync(PublicationEntity publication)
        {
            _context.Publications.Update(publication);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
