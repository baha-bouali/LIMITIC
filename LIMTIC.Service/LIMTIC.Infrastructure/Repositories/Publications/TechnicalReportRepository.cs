using LIMTIC.Domain.Abstractions.Publications;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories.Publications
{
    public class TechnicalReportRepository : ITechnicalReportRepository
    {
        private readonly AppDbContext _dbContext;

        public TechnicalReportRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TechnicalReportEntity?> GetByPublicationIdAsync(Guid publicationId)
        {
            return await _dbContext.TechnicalReports
                .FirstOrDefaultAsync(tr => tr.Id == publicationId); 
        }

        public async Task AddAsync(TechnicalReportEntity entity)
        {
            await _dbContext.TechnicalReports.AddAsync(entity);
        }

        public void Update(TechnicalReportEntity entity)
        {
            _dbContext.TechnicalReports.Update(entity);
        }

        public void Remove(TechnicalReportEntity entity)
        {
            _dbContext.TechnicalReports.Remove(entity);
        }
    }
}