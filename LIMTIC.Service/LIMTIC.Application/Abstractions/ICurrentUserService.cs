using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Abstractions
{
    /// <summary>
    /// Abstracts the identity of the authenticated user making the current request.
    /// Implemented in the Infrastructure layer via IHttpContextAccessor + JWT claims.
    /// Register as Scoped.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>The Id of the currently authenticated user (from the 'sub' / NameIdentifier claim).</summary>
        Guid? UserId { get; }

        /// <summary>The role of the currently authenticated user (from the 'role' claim).</summary>
        string? Role { get; }
    }
}