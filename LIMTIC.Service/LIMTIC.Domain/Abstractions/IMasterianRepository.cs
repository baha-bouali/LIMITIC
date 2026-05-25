using LIMTIC.Domain.Entities.Users;

namespace LIMTIC.Domain.Abstractions
{
    public interface IMasterianRepository
    {
        Task<MasterianEntity?> GetByUserIdAsync(Guid userId);
        Task<List<MasterianEntity>> GetAllAsync();
        Task<bool> AddAsync(MasterianEntity entity);
        Task<bool> UpdateAsync(MasterianEntity entity);
        Task<bool> DeleteAsync(MasterianEntity entity);
        Task<bool> ExistsAsync(Guid userId);
    }
}
