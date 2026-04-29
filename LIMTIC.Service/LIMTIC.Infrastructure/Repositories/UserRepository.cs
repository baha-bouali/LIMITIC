using LIMTIC.Application.Abstractions.Repositories;
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

        public async Task<User?> GetUserById(Guid userId)
        {
            return await _dbContext
                .Users
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
