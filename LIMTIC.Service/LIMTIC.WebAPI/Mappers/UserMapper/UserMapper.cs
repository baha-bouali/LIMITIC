using LIMTIC.Domain.Entities;
using LIMTIC.WebAPI.Models.UserManagement;

namespace LIMTIC.WebAPI.Mappers.UserMapper
{
    public class UserMapper : IUserMapper
    {
        public UserDto MapToUserDto(User user)
        {
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                Role = user.Role
            };
        }
    }
}
