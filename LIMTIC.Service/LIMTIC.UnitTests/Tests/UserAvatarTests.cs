using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
using LIMTIC.UnitTests.Base;
using System.Text;

namespace LIMTIC.UnitTests.Tests
{
    public class UserAvatarTests : BaseTests
    {
        private async Task<UserEntity> CreateUserAsync(string email)
        {
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Avatar",
                LastName = "Test",
                Email = email,
                PasswordHash = "hash",
                Role = UserRole.Visitor,
                IsActive = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };
            await UserRepository.AddUserAsync(user);
            return user;
        }

        [Fact]
        public async Task UpdateAvatar_ValidUser_StoresBlobName()
        {
            var user = await CreateUserAsync("avatar.upload@example.com");
            var fakeFile = Encoding.UTF8.GetBytes("fake image content");

            using var stream = new MemoryStream(fakeFile);
            var result = await UsersManagementService.UpdateUserAvatarAsync(
                user.Id, stream, "photo.jpg", "image/jpeg");

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Contains("avatars/", result.Data);
            Assert.Contains("photo.jpg", result.Data);

            // Verify the AvatarBlobName was persisted
            var updated = await UserRepository.GetUserByIdAsync(user.Id);
            Assert.Equal(result.Data, updated!.AvatarBlobName);
        }

        [Fact]
        public async Task UpdateAvatar_NonExistingUser_ReturnsFailure()
        {
            var fakeFile = Encoding.UTF8.GetBytes("fake image content");
            using var stream = new MemoryStream(fakeFile);

            var result = await UsersManagementService.UpdateUserAvatarAsync(
                Guid.NewGuid(), stream, "photo.jpg", "image/jpeg");

            Assert.False(result.Success);
            Assert.Equal("User not found", result.Message);
        }

        [Fact]
        public async Task UpdateAvatar_CalledTwice_OverwritesBlobName()
        {
            var user = await CreateUserAsync("avatar.overwrite@example.com");
            var fakeFile = Encoding.UTF8.GetBytes("fake image content");

            using var stream1 = new MemoryStream(fakeFile);
            var result1 = await UsersManagementService.UpdateUserAvatarAsync(
                user.Id, stream1, "first.jpg", "image/jpeg");
            Assert.True(result1.Success);

            using var stream2 = new MemoryStream(fakeFile);
            var result2 = await UsersManagementService.UpdateUserAvatarAsync(
                user.Id, stream2, "second.jpg", "image/jpeg");
            Assert.True(result2.Success);

            // Final blob name should reflect second upload
            var updated = await UserRepository.GetUserByIdAsync(user.Id);
            Assert.Equal(result2.Data, updated!.AvatarBlobName);
            Assert.Contains("second.jpg", updated.AvatarBlobName);
        }
    }
}
