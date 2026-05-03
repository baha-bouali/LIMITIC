using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Abstractions
{
    public interface ICurrentUserService
    {
        public Guid UserId { get; }
        public UserRole? Role { get; }
    }
}
