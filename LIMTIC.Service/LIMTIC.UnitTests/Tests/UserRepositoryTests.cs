using LIMTIC.Domain.Entities;
using LIMTIC.Domain.Enums;
using LIMTIC.UnitTests.Base;

namespace LIMTIC.UnitTests.Tests
{
    public class UserRepositoryTests : BaseTests
    {
        [Fact]
        public async Task AddUser_ShouldAddUserToDatabase()
        {
            // Arrange
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
                CreatedAt = DateTime.UtcNow,
            };

            var result = await UserRepository.AddUserAsync(user);
            Assert.True(result);
        }
    }
}
