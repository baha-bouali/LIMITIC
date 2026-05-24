using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
using LIMTIC.UnitTests.Base;

namespace LIMTIC.UnitTests.Tests
{
    public class GetUsersTests : BaseTests
    {
        private async Task<UserEntity> SeedUserAsync(
            string email, string firstName, string lastName,
            UserRole role = UserRole.Visitor, bool isActive = true)
        {
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = "hash",
                Role = role,
                IsActive = isActive,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };
            await UserRepository.AddUserAsync(user);
            return user;
        }

        // ── NO FILTERS ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetUsers_NoFilters_ReturnsAllSeededUsers()
        {
            await SeedUserAsync("getall.u1@test.com", "Alpha", "One");
            await SeedUserAsync("getall.u2@test.com", "Alpha", "Two");

            var result = await UsersManagementService.GetUsersAsync(null, null, null, 1, 100);

            Assert.True(result.Success);
            Assert.True(result.Data!.Total >= 2);
            Assert.True(result.Data.Items.Count >= 2);
        }

        // ── ROLE FILTER ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetUsers_FilterByRole_ReturnsOnlyMatchingRole()
        {
            await SeedUserAsync("getrole.admin@test.com", "Admin", "Filter", UserRole.Admin);
            await SeedUserAsync("getrole.visitor@test.com", "Visitor", "Filter", UserRole.Visitor);

            var result = await UsersManagementService.GetUsersAsync(UserRole.Admin, null, null, 1, 100);

            Assert.True(result.Success);
            Assert.All(result.Data!.Items, u => Assert.Equal(UserRole.Admin, u.Role));
        }

        // ── STATUS FILTER ─────────────────────────────────────────────────────────

        [Fact]
        public async Task GetUsers_FilterByActiveStatus_ReturnsOnlyActiveUsers()
        {
            await SeedUserAsync("getstatus.active@test.com", "Status", "Active", isActive: true);
            await SeedUserAsync("getstatus.inactive@test.com", "Status", "Inactive", isActive: false);

            var result = await UsersManagementService.GetUsersAsync(null, true, null, 1, 100);

            Assert.True(result.Success);
            Assert.All(result.Data!.Items, u => Assert.True(u.IsActive));
        }

        [Fact]
        public async Task GetUsers_FilterByInactiveStatus_ReturnsOnlyInactiveUsers()
        {
            await SeedUserAsync("getstatus.inactive2@test.com", "Status", "Inactive2", isActive: false);

            var result = await UsersManagementService.GetUsersAsync(null, false, null, 1, 100);

            Assert.True(result.Success);
            Assert.True(result.Data!.Items.Count >= 1);
            Assert.All(result.Data.Items, u => Assert.False(u.IsActive));
        }

        // ── SEARCH ────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetUsers_SearchByUniqueFirstName_ReturnsMatchingUser()
        {
            var unique = "Zxqpvuniq";
            await SeedUserAsync($"search.firstname@test.com", unique, "SearchTest");

            var result = await UsersManagementService.GetUsersAsync(null, null, unique, 1, 100);

            Assert.True(result.Success);
            Assert.True(result.Data!.Items.All(u => u.FirstName.Contains(unique, StringComparison.OrdinalIgnoreCase)));
            Assert.True(result.Data.Items.Count >= 1);
        }

        [Fact]
        public async Task GetUsers_SearchByUniqueEmail_ReturnsMatchingUser()
        {
            var uniqueDomain = "uniquedomain99.com";
            await SeedUserAsync($"user@{uniqueDomain}", "EmailSearch", "User");

            var result = await UsersManagementService.GetUsersAsync(null, null, uniqueDomain, 1, 100);

            Assert.True(result.Success);
            Assert.True(result.Data!.Items.Count >= 1);
            Assert.All(result.Data.Items, u =>
                Assert.True(u.Email.Contains(uniqueDomain, StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public async Task GetUsers_SearchWithNoMatch_ReturnsEmptyItems()
        {
            var result = await UsersManagementService.GetUsersAsync(null, null, "NOMATCH_XYZZY_99999", 1, 100);

            Assert.True(result.Success);
            Assert.Empty(result.Data!.Items);
            Assert.Equal(0, result.Data.Total);
        }

        // ── PAGINATION ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetUsers_Pagination_ReturnsCorrectPageSize()
        {
            var domain = "paginate77.com";
            await SeedUserAsync($"p1@{domain}", "Page", "One");
            await SeedUserAsync($"p2@{domain}", "Page", "Two");
            await SeedUserAsync($"p3@{domain}", "Page", "Three");

            var page1 = await UsersManagementService.GetUsersAsync(null, null, domain, 1, 2);
            Assert.True(page1.Success);
            Assert.Equal(2, page1.Data!.Items.Count);
            Assert.Equal(3, page1.Data.Total);
            Assert.Equal(1, page1.Data.Page);
            Assert.Equal(2, page1.Data.Limit);

            var page2 = await UsersManagementService.GetUsersAsync(null, null, domain, 2, 2);
            Assert.True(page2.Success);
            Assert.Equal(1, page2.Data!.Items.Count);
            Assert.Equal(3, page2.Data.Total);
        }

        // ── COUNTS ────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetUsers_ReturnsCounts_ContainsSeededRoles()
        {
            await SeedUserAsync("counts.admin@test.com", "Counts", "Admin", UserRole.Admin);
            await SeedUserAsync("counts.visitor@test.com", "Counts", "Visitor", UserRole.Visitor);

            var result = await UsersManagementService.GetUsersAsync(null, null, null, 1, 100);

            Assert.True(result.Success);
            Assert.True(result.Data!.Counts.ContainsKey(UserRole.Admin));
            Assert.True(result.Data.Counts.ContainsKey(UserRole.Visitor));
        }

        // ── PAGE & LIMIT REFLECTED IN RESULT ──────────────────────────────────────

        [Fact]
        public async Task GetUsers_PageAndLimitReflectedInResult()
        {
            var result = await UsersManagementService.GetUsersAsync(null, null, null, 3, 5);

            Assert.True(result.Success);
            Assert.Equal(3, result.Data!.Page);
            Assert.Equal(5, result.Data.Limit);
        }
    }
}
