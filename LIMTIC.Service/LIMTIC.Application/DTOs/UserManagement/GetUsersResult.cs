using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.DTOs.UserManagement
{
    public class GetUsersResult
    {
        public List<UserDto> Items { get; set; } = [];
        public int Total { get; set; }
        public Dictionary<UserRole, int> Counts { get; set; } = [];
        public int Page { get; set; }
        public int Limit { get; set; }
    }
}
