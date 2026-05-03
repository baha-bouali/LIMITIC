using LIMTIC.Domain.Entities;

namespace LIMTIC.Domain.Abstractions
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> AddUserAsync(User user);
        Task<bool> UpdateUserPasswordAsync(User user, string password);
        Task<bool> UpdateUserAsync(User user);


    }
}
