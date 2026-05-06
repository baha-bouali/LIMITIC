using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserEntity?> GetUserByIdAsync(Guid userId)
        {
            return await _dbContext
                .Users
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<UserEntity?> GetUserByEmailAsync(string email)
        {
            return await _dbContext
                .Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> AddUserAsync(UserEntity user)
        {
            await _dbContext.Users.AddAsync(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
