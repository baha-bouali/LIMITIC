using LIMTIC.Domain.Abstractions.Files;
using LIMTIC.Domain.Entities.Files;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories.Files
{
    public class PublicationFilesRepository : IPublicationFilesRepository
    {
        private readonly AppDbContext _context;

        public PublicationFilesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PublicationFileEntity file)
        {
            await _context.PublicationFiles.AddAsync(file);
        }

        public async Task AddRangeAsync(IEnumerable<PublicationFileEntity> files)
        {
            await _context.PublicationFiles.AddRangeAsync(files);
        }

        public async Task<PublicationFileEntity?> GetByIdAsync(Guid id)
        {
            return await _context.PublicationFiles
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<PublicationFileEntity>> GetByPublicationIdAsync(Guid publicationId)
        {
            return await _context.PublicationFiles
                .Where(x => x.PublicationId == publicationId)
                .ToListAsync();
        }

        public async Task DeleteAsync(PublicationFileEntity file)
        {
            _context.PublicationFiles.Remove(file);
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);

            if (entity != null)
                _context.PublicationFiles.Remove(entity);
        }

        public async Task DeleteByPublicationIdAsync(Guid publicationId)
        {
            var files = await _context.PublicationFiles
                .Where(x => x.PublicationId == publicationId)
                .ToListAsync();

            _context.PublicationFiles.RemoveRange(files);
        }
    }
}
