using LIMTIC.Domain.Abstractions.Publications;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories.Publications
{
    public class InternationalConferenceRepository : IInternationalConferenceRepository
    {
        private readonly AppDbContext _context;

        public InternationalConferenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<InternationalConferenceEntity?> GetByPublicationIdAsync(Guid publicationId)
        {
            return await _context.InternationalConferences
                .FirstOrDefaultAsync(ic => ic.Id == publicationId);
        }

        public async Task AddAsync(InternationalConferenceEntity entity)
        {
            await _context.InternationalConferences.AddAsync(entity);
        }

        public void Update(InternationalConferenceEntity entity)
        {
            _context.InternationalConferences.Update(entity);
        }

        public void Remove(InternationalConferenceEntity entity)
        {
            _context.InternationalConferences.Remove(entity);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}