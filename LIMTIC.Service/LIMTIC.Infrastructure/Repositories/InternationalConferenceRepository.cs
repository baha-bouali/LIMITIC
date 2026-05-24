using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class InternationalConferenceRepository : IInternationalConferenceRepository
    {
        private readonly AppDbContext _context;

        public InternationalConferenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<InternationalConferenceEntity?> GetByPublicationIdAsync(Guid publicationId, CancellationToken ct = default)
        {
            // The Id of the specific entity acts as a primary key constraint corresponding 
            // to the parent PublicationId, based on your Entity Framework configurations.
            return await _context.InternationalConferences
                .FirstOrDefaultAsync(ic => ic.Id == publicationId, ct);
        }

        public async Task AddAsync(InternationalConferenceEntity entity, CancellationToken ct = default)
        {
            await _context.InternationalConferences.AddAsync(entity, ct);
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