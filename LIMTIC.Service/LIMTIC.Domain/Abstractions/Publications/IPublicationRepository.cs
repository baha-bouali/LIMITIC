using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Domain.Abstractions.Publications
{
    public interface IPublicationRepository
    {
        Task<PublicationEntity?> GetByIdAsync(Guid id);

        Task<PublicationEntity?> GetByIdWithDetailsAsync(Guid id);

        Task<(IEnumerable<PublicationEntity> Items, int TotalCount)> GetFilteredAsync(
            PublicationType? type,
            PublicationStatus? status,
            PublicationVisibility? visibility,
            Guid? userId,
            Guid? researchAxisId,
            int? year,
            string? search,
            int page,
            int limit);

        Task AddAsync(PublicationEntity entity);

        void Update(PublicationEntity entity);

        void Remove(PublicationEntity entity);
    }
}
