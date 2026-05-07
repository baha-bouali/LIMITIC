using LIMTIC.Domain.Entities.Users;

namespace LIMTIC.Domain.Abstractions
{
    public interface IUserRepository
    {
        Task<UserEntity?> GetUserByIdAsync(Guid userId);
        Task<UserEntity?> GetUserByEmailAsync(string email);
        Task<bool> AddUserAsync(UserEntity user);
        Task<bool> UpdateUserAsync(UserEntity user);
    }
}
