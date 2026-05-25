using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Domain.Abstractions
{
    public interface IPublicationRepository
    {
        // Read
        Task<PublicationEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>Same as GetByIdAsync but eager-loads every navigation property (type-specific child, User, ResearchAxis).</summary>
        Task<PublicationEntity?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);

        Task<IEnumerable<PublicationEntity>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

        Task<IEnumerable<PublicationEntity>> GetByUserIdAndStatusAsync(
            Guid userId, PublicationStatus status, CancellationToken ct = default);

        Task<IEnumerable<PublicationEntity>> GetByStatusAndVisibilityAsync(
            PublicationStatus status, PublicationVisibility visibility, CancellationToken ct = default);

        Task<IEnumerable<PublicationEntity>> GetByResearchAxisIdAsync(Guid researchAxisId, CancellationToken ct = default);

        Task<IEnumerable<PublicationEntity>> GetByTypeAsync(PublicationType type, CancellationToken ct = default);

        Task<(IEnumerable<PublicationEntity> Items, int TotalCount)> GetFilteredAsync(
            PublicationType? type,
            PublicationStatus? status,
            PublicationVisibility? visibility,
            Guid? userId,
            Guid? researchAxisId,
            int? year,
            string? search,
            int page,
            int pageSize,
            CancellationToken ct = default);

        Task<int> CountByStatusAndVisibilityAsync(
            PublicationStatus status, PublicationVisibility visibility, CancellationToken ct = default);

        // Write
        Task AddAsync(PublicationEntity entity, CancellationToken ct = default);
        void Update(PublicationEntity entity);
        void Remove(PublicationEntity entity);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
