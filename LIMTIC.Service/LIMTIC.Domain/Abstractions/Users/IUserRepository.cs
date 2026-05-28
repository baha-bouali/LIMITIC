using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Domain.Abstractions.Users
{
    public interface IUserRepository
    {
        Task<UserEntity?> GetUserByIdAsync(Guid userId);
        Task<UserEntity?> GetUserByEmailAsync(string email);
        Task<bool> AddUserAsync(UserEntity user);
        Task<bool> UpdateUserAsync(UserEntity user);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<(List<UserEntity> Items, int Total)> GetUsersAsync(
            UserRole? role, bool? isActive, string? search, int page, int limit);
    }
}
