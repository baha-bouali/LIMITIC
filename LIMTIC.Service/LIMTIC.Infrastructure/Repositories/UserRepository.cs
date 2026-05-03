using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities;
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

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _dbContext
                .Users
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbContext
                .Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> AddUserAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateUserPassword(User user, string password)
        {
            user.PasswordHash = password;
            _dbContext.Users.Update(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateUserAsync(User user)
        {
            _dbContext.Users.Update(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
