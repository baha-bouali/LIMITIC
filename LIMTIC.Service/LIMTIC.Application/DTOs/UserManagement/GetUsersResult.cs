using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.DTOs.UserManagement
{
    public class GetUsersResult
    {
        public List<UserDto> Items { get; set; } = [];
        public int Total { get; set; }
    }
}
