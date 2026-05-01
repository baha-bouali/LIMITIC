using LIMTIC.Domain.Entities;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using System.Net.Http.Json;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class UsersCrudTests : BaseE2ETests
    {
        public UsersCrudTests(PostgresFixture fixture) : base(fixture)
        {
        }

        // Add your test methods here
        [Fact]
        public async Task AddAndGetUserE2ETest()
        {
            // Steps:
            // 1. Create a new user object with valid data
            // 2. Send a POST request to the API endpoint to add the user
            // 3. Assert that the response indicates success and the user was added
            // 4. Send a GET request to the API endpoint to retrieve the user by ID

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "John.Doe@example.com",
                PasswordHash = "hashedpassword",
                Role = UserRole.Admin,
                AvatarBlobName = null,
                IsActive = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow,
            };

            var response = await Client.AddUser(user);
            Assert.True(response, "Failed to add user");

            // 4. Send a GET request to the API endpoint to retrieve the user by ID
            var retrievedUser = await Client.GetUserById(user.Id);

            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Id, retrievedUser.Id);
            Assert.Equal(user.FirstName, retrievedUser.FirstName);
            Assert.Equal(user.LastName, retrievedUser.LastName);
            Assert.Equal(user.Email, retrievedUser.Email);
        }
    }
}
