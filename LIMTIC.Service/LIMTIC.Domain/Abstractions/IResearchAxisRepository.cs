using LIMTIC.Domain.Entities.ResearchAxis;

namespace LIMTIC.Domain.Abstractions
{
    public interface IResearchAxisRepository
    {
        Task<List<ResearchAxisEntity>> GetAllAsync();
        Task<ResearchAxisEntity?> GetByIdAsync(Guid id);
        Task<List<ResearchAxisEntity>> GetByIdsAsync(List<Guid> ids);
        Task<bool> AddAsync(ResearchAxisEntity entity);
        Task<bool> UpdateAsync(ResearchAxisEntity entity);
        Task<bool> DeleteAsync(ResearchAxisEntity entity);
        Task<bool> ExistsAsync(Guid id);
    }
}
