using LIMTIC.Domain.Entities;

namespace LIMTIC.Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserById(Guid userId);

        Task<bool> AddUser(User user);
    }
}
