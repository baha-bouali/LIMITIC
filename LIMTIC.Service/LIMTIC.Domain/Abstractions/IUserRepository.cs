using LIMTIC.Domain.Entities;

namespace LIMTIC.Domain.Abstractions
{
    public interface IUserRepository
    {
        Task<User?> GetUserById(Guid userId);
        Task<bool> AddUser(User user);
        Task<User?> GetUserByEmail(string email);
    }
}
