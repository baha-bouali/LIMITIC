using LIMTIC.Application.Abstractions;
using LIMTIC.Domain.Enums;
using System.Security.Claims;

namespace LIMTIC.UnitTests.Helpers
{
    /// <summary>
    /// Stub implementation of ICurrentUserService for unit tests.
    /// Acts as a SuperAdmin so all profile service authorization checks pass.
    /// </summary>
    public class TestCurrentUserService : ICurrentUserService
    {
        public Guid? UserId => Guid.Empty;
        public string? Role => UserRole.SuperAdmin.ToString();
    }
}
