using LIMTIC.Domain.Entities.Users;

namespace LIMTIC.Domain.Abstractions
{
    public interface IPhDStudentRepository
    {
        Task<PhDStudentEntity?> GetByUserIdAsync(Guid userId);
        Task<List<PhDStudentEntity>> GetAllAsync();
        Task<bool> AddAsync(PhDStudentEntity entity);
        Task<bool> UpdateAsync(PhDStudentEntity entity);
        Task<bool> DeleteAsync(PhDStudentEntity entity);
        Task<bool> ExistsAsync(Guid userId);
    }
}
