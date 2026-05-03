using LIMTIC.Application.Abstractions;
using LIMTIC.Domain.Enums;
using System.Security.Claims;

namespace LIMTIC.WebAPI.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId => Guid.TryParse(
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId)
                ? userId : Guid.Empty;

        public UserRole? Role => Enum.TryParse<UserRole>(
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value, out var userRole)
                ? userRole : null;
    }
}
