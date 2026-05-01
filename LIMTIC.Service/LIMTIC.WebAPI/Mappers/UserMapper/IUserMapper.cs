using LIMTIC.Domain.Entities;
using LIMTIC.WebAPI.Models.UserManagement;

namespace LIMTIC.WebAPI.Mappers.UserMapper
{
    public interface IUserMapper
    {
        public UserDto MapToUserDto(User user);
    }
}
