using LIMTIC.Application.Abstractions;

namespace LIMTIC.UnitTests.Helpers
{
    /// <summary>
    /// A mutable in-memory stub for <see cref="ICurrentUserService"/>.
    /// Tests retrieve this via <c>BaseTests.CurrentUserService</c> and set
    /// <see cref="UserId"/> / <see cref="Role"/> directly — no Moq required.
    ///
    /// Default: authenticated SuperAdmin (matches the existing BaseTests behaviour
    /// so existing non-publication tests continue to pass unchanged).
    /// </summary>
    public sealed class TestCurrentUserService : ICurrentUserService
    {
        /// <summary>
        /// Set to <see cref="null"/> to simulate an unauthenticated request.
        /// </summary>
        public Guid? UserId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Set to <c>"Admin"</c>, <c>"User"</c>, or <c>null</c> as needed by the test.
        /// Defaults to <c>"SuperAdmin"</c> so existing profile / axis tests still pass.
        /// </summary>
        public string? Role { get; set; } = "SuperAdmin";

        // ── ICurrentUserService ────────────────────────────────────────────────

        Guid? ICurrentUserService.UserId => UserId;
        string? ICurrentUserService.Role => Role;
    }
}