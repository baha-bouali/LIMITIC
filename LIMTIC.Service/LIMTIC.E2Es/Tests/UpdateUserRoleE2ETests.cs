using System.Net.Http.Headers;
using System.Net.Http.Json;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;
using LIMTIC.WebAPI.Models.UserManagement.CreateUser;
using LIMTIC.WebAPI.Models.UserManagement.UpdateUserRole;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class UpdateUserRoleE2ETests : BaseE2ETests
    {
        public UpdateUserRoleE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture) : base(dbfixture: dbfixture, mailHogFixture: mailFixture, useShortTokenExpiry: true)
        {
        }

        [Fact]
        public async Task UpdateUserRole_ToResearcher_E2E()
        {
            var token = await LoginAsSuperAdmin();

            var createUserRequest = new CreateUserRequest
            {
                FirstName = "E2E",
                LastName = "Research",
                Email = "e2e.research@example.com",
                Password = "password",
                IsActive = true
            };

            var createResponse = await Client.AddUser(createUserRequest, token);
            Assert.NotNull(createResponse);

            var updateRequest = new UpdateUserRoleRequest
            {
                Role = UserRole.Researcher,
                Rank = "Professor",
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345",
                ResearchAxisIds = new List<Guid>()
            };

            // ensure authorization header is set for the PUT request
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await Client.PutAsJsonAsync($"api/users/updateRole/{createResponse.User.Id}", updateRequest);
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}
