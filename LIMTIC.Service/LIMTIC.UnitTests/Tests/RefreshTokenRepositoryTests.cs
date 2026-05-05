using LIMTIC.Domain.Entities;
using LIMTIC.Domain.Enums;
using LIMTIC.UnitTests.Base;

namespace LIMTIC.UnitTests.Tests
{
    public class RefreshTokenRepositoryTests : BaseTests
    {
        [Fact]
        public async Task AddRefreshToken_ThenGetRefreshToken_ReturnsToken()
        {
            // 1. Create a new user object with valid data
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Email = "test.user@example.com",
                PasswordHash = "hashed",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            await UserRepository.AddUserAsync(user);

            // 2. Create a new refresh token for the user and add it to the repository
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Guid.NewGuid().ToString(),
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            int addCnt = await RefreshTokenRepository.AddRefreshTokenAsync(refreshToken);
            Assert.Equal(1, addCnt);

            // 3. Retrieve the refresh token by its token string and assert that it matches the original token
            var fetched = await RefreshTokenRepository.GetRefreshTokenAsync(refreshToken.Token);
            Assert.NotNull(fetched);
            Assert.Equal(refreshToken.Token, fetched!.Token);
            Assert.Equal(user.Id, fetched.UserId);
        }
    }
}
