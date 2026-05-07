using LIMTIC.Application.DTOs.UserManagement;
using LIMTIC.Domain.Entities.Users;

namespace LIMTIC.Application.Mappers.UserMapper
{
    public class UserMapper : IUserMapper
    {
        public UserDto MapToUserDto(UserEntity user)
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
