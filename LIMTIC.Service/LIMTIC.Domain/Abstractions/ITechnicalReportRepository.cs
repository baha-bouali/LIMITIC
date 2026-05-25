using LIMTIC.Domain.Entities.Publications;

namespace LIMTIC.Domain.Abstractions
{
    public interface ITechnicalReportRepository
    {
        Task<TechnicalReportEntity?> GetByPublicationIdAsync(Guid publicationId, CancellationToken ct = default);
        Task AddAsync(TechnicalReportEntity entity, CancellationToken ct = default);
        void Update(TechnicalReportEntity entity);
        void Remove(TechnicalReportEntity entity);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
