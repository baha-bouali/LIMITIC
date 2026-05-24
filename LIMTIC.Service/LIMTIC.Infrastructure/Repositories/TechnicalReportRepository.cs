using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class TechnicalReportRepository : ITechnicalReportRepository
    {
        private readonly AppDbContext _dbContext;

        public TechnicalReportRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TechnicalReportEntity?> GetByPublicationIdAsync(Guid publicationId, CancellationToken ct = default)
        {
            return await _dbContext.TechnicalReports
                .FirstOrDefaultAsync(tr => tr.Id == publicationId, ct); 
                // Note: If PublicationId is a separate property on TechnicalReportEntity (e.g. inheritance or foreign key), 
                // adjust `tr.Id == publicationId` to `tr.PublicationId == publicationId` accordingly.
        }

        public async Task AddAsync(TechnicalReportEntity entity, CancellationToken ct = default)
        {
            await _dbContext.TechnicalReports.AddAsync(entity, ct);
        }

        public void Update(TechnicalReportEntity entity)
        {
            _dbContext.TechnicalReports.Update(entity);
        }

        public void Remove(TechnicalReportEntity entity)
        {
            _dbContext.TechnicalReports.Remove(entity);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        Task<TechnicalReportEntity?> ITechnicalReportRepository.GetByPublicationIdAsync(Guid publicationId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}