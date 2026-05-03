using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
using LIMTIC.UnitTests.Base;

namespace LIMTIC.UnitTests.Tests
{
    public class UserRepositoryTests : BaseTests
    {
        [Fact]
        public async Task AddAndGetUserByIdThenByEmailTest()
        {
            // Steps: 
            // 1. Create a new user object with valid data
            // 2. Send the user object to the repository method responsible for adding users
            // 3. Retrieve the user by id and assert that the retrieved user matches the original user 
            // 4. Retrieve the user by email and assert that the retrieved user matches the original user

            // 1. Create a new user object with valid data
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PasswordHash = "hashedpassword",
                Role = UserRole.Admin,
                AvatarBlobName = null,
                IsActive = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow,
            };

            // 2. Send the user object to the repository method responsible for adding users and assert success
            var result = await UserRepository.AddUserAsync(user);
            Assert.True(result);

            // 3. Retrieve the user by id and assert that the retrieved user matches the original user 
            var retrievedUser = await UserRepository.GetUserByIdAsync(user.Id);
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Id, retrievedUser.Id);
            Assert.Equal(user.FirstName, retrievedUser.FirstName);
            Assert.Equal(user.LastName, retrievedUser.LastName);

            // 4. Retrieve the user by email and assert that the retrieved user matches the original user
            retrievedUser = await UserRepository.GetUserByEmailAsync(user.Email);
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Id, retrievedUser.Id);
            Assert.Equal(user.FirstName, retrievedUser.FirstName);
            Assert.Equal(user.LastName, retrievedUser.LastName);
        }
    }
}
