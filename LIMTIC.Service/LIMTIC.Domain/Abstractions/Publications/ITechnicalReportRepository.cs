using LIMTIC.Domain.Entities.Publications;

namespace LIMTIC.Domain.Abstractions.Publications
{
    public interface ITechnicalReportRepository
    {
        Task<TechnicalReportEntity?> GetByPublicationIdAsync(Guid publicationId);
        Task AddAsync(TechnicalReportEntity entity);
        void Update(TechnicalReportEntity entity);
        void Remove(TechnicalReportEntity entity);
    }
}
