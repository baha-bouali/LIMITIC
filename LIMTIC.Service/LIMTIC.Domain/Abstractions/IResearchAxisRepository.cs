using LIMTIC.Domain.Entities.ResearchAxis;

namespace LIMTIC.Domain.Abstractions
{
    public interface IResearchAxisRepository
    {
        Task<List<ResearchAxisEntity>> GetByIdsAsync(List<Guid> ids);
    }
}
