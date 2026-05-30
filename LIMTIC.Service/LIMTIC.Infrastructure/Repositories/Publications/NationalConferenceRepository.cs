using LIMTIC.Domain.Abstractions.Publications;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories.Publications
{
    public class NationalConferenceRepository : INationalConferenceRepository
    {
        private readonly AppDbContext _context;

        public NationalConferenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NationalConferenceEntity?> GetByPublicationIdAsync(Guid publicationId)
        {
            return await _context.NationalConferences
                .FirstOrDefaultAsync(nc => nc.Id == publicationId);
        }

        public async Task AddAsync(NationalConferenceEntity entity)
        {
            await _context.NationalConferences.AddAsync(entity);
        }

        public void Update(NationalConferenceEntity entity)
        {
            _context.NationalConferences.Update(entity);
        }

        public void Remove(NationalConferenceEntity entity)
        {
            _context.NationalConferences.Remove(entity);
        }
    }
}