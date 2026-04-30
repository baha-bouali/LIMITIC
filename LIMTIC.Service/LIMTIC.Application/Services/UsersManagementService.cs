using LIMTIC.Application.Abstractions.Repositories;
using LIMTIC.Domain.Entities;

namespace LIMTIC.Application.Services
{
    public class UsersManagementService
    {
        private readonly IUserRepository userRepository;

        public UsersManagementService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<bool> AddUser(User user)
        {
            return await userRepository.AddUser(user);
        }

        public async Task<User?> GetUser(Guid id)
        {
            return await userRepository.GetUserById(id);
        }
    }
}
