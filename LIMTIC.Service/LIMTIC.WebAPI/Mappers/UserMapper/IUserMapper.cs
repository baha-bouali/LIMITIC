using LIMTIC.Application.DTOs.UserManagement;
using LIMTIC.Domain.Entities.Users;

namespace LIMTIC.WebAPI.Mappers.UserMapper
{
    public interface IUserMapper
    {
        public UserDto MapToUserDto(UserEntity user);
    }
}
