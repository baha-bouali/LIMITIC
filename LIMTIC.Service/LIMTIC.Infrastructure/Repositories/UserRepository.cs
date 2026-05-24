using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
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
            => await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

        public async Task<UserEntity?> GetUserByEmailAsync(string email)
            => await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<bool> AddUserAsync(UserEntity user)
        {
            await _dbContext.Users.AddAsync(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserAsync(UserEntity user)
        {
            _dbContext.Users.Update(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<(List<UserEntity> Items, int Total, Dictionary<UserRole, int> Counts)> GetUsersAsync(
            UserRole? role, bool? isActive, string? search, int page, int limit)
        {
            var query = _dbContext.Users.AsQueryable();

            if (role.HasValue)
                query = query.Where(u => u.Role == role.Value);

            if (isActive.HasValue)
                query = query.Where(u => u.IsActive == isActive.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(lower) ||
                    u.LastName.ToLower().Contains(lower) ||
                    u.Email.ToLower().Contains(lower));
            }

            var counts = await _dbContext.Users
                .GroupBy(u => u.Role)
                .Select(g => new { Role = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Role, x => x.Count);

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (items, total, counts);
        }
    }
}
