using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class NationalConferenceRepository : INationalConferenceRepository
    {
        private readonly AppDbContext _context;

        public NationalConferenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NationalConferenceEntity?> GetByPublicationIdAsync(Guid publicationId, CancellationToken ct = default)
        {
            return await _context.NationalConferences
                .FirstOrDefaultAsync(nc => nc.Id == publicationId, ct);
        }

        public async Task AddAsync(NationalConferenceEntity entity, CancellationToken ct = default)
        {
            await _context.NationalConferences.AddAsync(entity, ct);
        }

        public void Update(NationalConferenceEntity entity)
        {
            _context.NationalConferences.Update(entity);
        }

        public void Remove(NationalConferenceEntity entity)
        {
            _context.NationalConferences.Remove(entity);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}