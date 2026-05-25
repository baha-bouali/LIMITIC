using LIMTIC.Domain.Entities.Users;

namespace LIMTIC.Domain.Abstractions
{
    public interface IResearcherRepository
    {
        Task<ResearcherEntity?> GetByUserIdAsync(Guid userId);
        Task<List<ResearcherEntity>> GetAllAsync();
        Task<bool> AddAsync(ResearcherEntity entity);
        Task<bool> UpdateAsync(ResearcherEntity entity);
        Task<bool> DeleteAsync(ResearcherEntity entity);
        Task<bool> ExistsAsync(Guid userId);
    }
}
