using LIMTIC.Application.DTOs.UserManagement;

namespace LIMTIC.WebAPI.Models.UserManagement.GetUsers
{
    public class GetUsersResponse : BaseResponse
    {
        public List<UserDto> Items { get; set; } = [];
        public int Total { get; set; }
        public Dictionary<string, int> Counts { get; set; } = [];
        public int Page { get; set; }
        public int Limit { get; set; }
    }
}
