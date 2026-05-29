using LIMTIC.Application.Contracts.Queries.Users;
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

            var result = await UsersManagementService.GetUsersAsync(new GetUsersQuery());

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

            var query = new GetUsersQuery
            {
                Role = UserRole.Admin,
                Page = 1,
                Limit = 100
            };
            var result = await UsersManagementService.GetUsersAsync(query);

            Assert.True(result.Success);
            Assert.All(result.Data!.Items, u => Assert.Equal(UserRole.Admin, u.Role));
        }

        // ── STATUS FILTER ─────────────────────────────────────────────────────────

        [Fact]
        public async Task GetUsers_FilterByActiveStatus_ReturnsOnlyActiveUsers()
        {
            await SeedUserAsync("getstatus.active@test.com", "Status", "Active", isActive: true);
            await SeedUserAsync("getstatus.inactive@test.com", "Status", "Inactive", isActive: false);

            var result = await UsersManagementService.GetUsersAsync(new GetUsersQuery());

            Assert.True(result.Success);
            Assert.All(result.Data!.Items, u => Assert.True(u.IsActive));
        }

        [Fact]
        public async Task GetUsers_FilterByInactiveStatus_ReturnsOnlyInactiveUsers()
        {
            await SeedUserAsync("getstatus.inactive2@test.com", "Status", "Inactive2", isActive: false);

            var result = await UsersManagementService.GetUsersAsync(new GetUsersQuery());

            Assert.True(result.Success);
            Assert.True(result.Data!.Items.Count >= 1);
            Assert.All(result.Data.Items, u => Assert.False(u.IsActive));
        }

        // ── PAGINATION ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetUsers_Pagination_ReturnsCorrectPageSize()
        {
            var domain = "paginate77.com";
            await SeedUserAsync($"p1@{domain}", "Page", "One");
            await SeedUserAsync($"p2@{domain}", "Page", "Two");
            await SeedUserAsync($"p3@{domain}", "Page", "Three");

            var query = new GetUsersQuery
            {
                Page = 1,
                Limit = 2
            };
            var page1 = await UsersManagementService.GetUsersAsync(query);
            Assert.True(page1.Success);
            Assert.Equal(2, page1.Data!.Items.Count);
            Assert.Equal(3, page1.Data.Total);

            query.Page = 2;
            var page2 = await UsersManagementService.GetUsersAsync(query);
            Assert.True(page2.Success);
            Assert.Equal(1, page2.Data!.Items?.Count);
            Assert.Equal(3, page2.Data.Total);
        }
    }
}
