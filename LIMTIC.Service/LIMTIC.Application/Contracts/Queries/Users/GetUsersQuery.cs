using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Contracts.Queries.Users
{
    public class GetUsersQuery
    {
        public UserRole? Role { get; set; }
        
        public bool? IsActive { get; set; } 

        public string? Q { get; set; }

        public int Page { get; set; } = 1;

        public int Limit { get; set; } = 20;
    }
}
