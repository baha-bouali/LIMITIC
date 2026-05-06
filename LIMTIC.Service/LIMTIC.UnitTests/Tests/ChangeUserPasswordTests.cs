using LIMTIC.Domain.Entities;
using LIMTIC.Domain.Enums;
using LIMTIC.UnitTests.Base;


namespace LIMTIC.UnitTests.Tests
{
    public class ChangeUserPasswordTests : BaseTests
    {
        private static User BuildUser(string email = "jane.doe@example.com", string passwordHash = "old_hashed_password") => new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Doe",
            Email = email,
            PasswordHash = passwordHash,
            Role = UserRole.Admin,
            AvatarBlobName = null,
            IsActive = true,
            CreatedBy = Guid.NewGuid(),
            CreatedAtUtc = DateTime.UtcNow,
        };

        [Fact]
        public async Task ChangeUserPassword_ValidUser_PasswordHashIsUpdated()
        {
            // Steps:
            // 1. Create and add a user to the repository
            // 2. Update the user's password
            // 3. Retrieve the user and assert the password hash was updated

            var user = BuildUser(email: "change.valid@example.com");
            var added = await UserRepository.AddUserAsync(user);
            Assert.True(added);

            const string newPasswordHash = "new_hashed_password";
            user.PasswordHash = newPasswordHash;
            var result = await UserRepository.UpdateUserAsync(user);
            Assert.True(result);

            var retrievedUser = await UserRepository.GetUserByEmailAsync(user.Email);
            Assert.NotNull(retrievedUser);
            Assert.Equal(newPasswordHash, retrievedUser.PasswordHash);
        }

        [Fact]
        public async Task ChangeUserPassword_OldPasswordHashNoLongerValid_AfterUpdate()
        {
            // Steps:
            // 1. Create and add a user with a known password hash
            // 2. Update to a new password hash
            // 3. Retrieve the user and assert the old hash is gone

            const string oldPasswordHash = "old_hashed_password";
            var user = BuildUser(email: "change.old.gone@example.com", passwordHash: oldPasswordHash);
            var added = await UserRepository.AddUserAsync(user);
            Assert.True(added);

            user.PasswordHash = "new_hashed_password";
            var result = await UserRepository.UpdateUserAsync(user);
            Assert.True(result);

            var retrievedUser = await UserRepository.GetUserByEmailAsync(user.Email);
            Assert.NotNull(retrievedUser);
            Assert.NotEqual(oldPasswordHash, retrievedUser.PasswordHash);
        }

        [Fact]
        public async Task ChangeUserPassword_DoesNotAffectOtherUserFields()
        {
            // Steps:
            // 1. Create and add a user with known field values
            // 2. Update only the password hash
            // 3. Retrieve the user and assert all other fields remain unchanged

            var user = BuildUser(email: "change.fields@example.com");
            var added = await UserRepository.AddUserAsync(user);
            Assert.True(added);

            user.PasswordHash = "new_hashed_password";
            var result = await UserRepository.UpdateUserAsync(user);
            Assert.True(result);

            var retrievedUser = await UserRepository.GetUserByEmailAsync(user.Email);
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Id, retrievedUser.Id);
            Assert.Equal(user.FirstName, retrievedUser.FirstName);
            Assert.Equal(user.LastName, retrievedUser.LastName);
            Assert.Equal(user.Email, retrievedUser.Email);
            Assert.Equal(user.Role, retrievedUser.Role);
            Assert.Equal(user.IsActive, retrievedUser.IsActive);
        }

        [Fact]
        public async Task ChangeUserPassword_CanBeChangedMultipleTimes()
        {
            // Steps:
            // 1. Create and add a user
            // 2. Change the password a first time and assert the update
            // 3. Change the password a second time and assert the update

            var user = BuildUser(email: "change.multiple@example.com");
            var added = await UserRepository.AddUserAsync(user);
            Assert.True(added);

            const string firstNewHash = "first_new_hashed_password";
            user.PasswordHash = firstNewHash;
            var firstResult = await UserRepository.UpdateUserAsync(user);
            Assert.True(firstResult);

            var afterFirstChange = await UserRepository.GetUserByEmailAsync(user.Email);
            Assert.NotNull(afterFirstChange);
            Assert.Equal(firstNewHash, afterFirstChange.PasswordHash);

            const string secondNewHash = "second_new_hashed_password";
            user.PasswordHash = secondNewHash;
            var secondResult = await UserRepository.UpdateUserAsync(afterFirstChange);
            Assert.True(secondResult);

            var afterSecondChange = await UserRepository.GetUserByEmailAsync(user.Email);
            Assert.NotNull(afterSecondChange);
            Assert.Equal(secondNewHash, afterSecondChange.PasswordHash);
        }

        [Fact]
        public async Task ChangeUserPassword_DoesNotAffectOtherUsersPassword()
        {
            // Steps:
            // 1. Create and add two distinct users
            // 2. Update the password of the first user only
            // 3. Assert the second user's password hash is unchanged

            const string sharedOriginalHash = "shared_original_hash";
            var userOne = BuildUser(email: "change.userone@example.com", passwordHash: sharedOriginalHash);
            var userTwo = BuildUser(email: "change.usertwo@example.com", passwordHash: sharedOriginalHash);

            Assert.True(await UserRepository.AddUserAsync(userOne));
            Assert.True(await UserRepository.AddUserAsync(userTwo));

            userOne.PasswordHash = "userone_new_hash";
            var result = await UserRepository.UpdateUserAsync(userOne);
            Assert.True(result);

            var retrievedUserTwo = await UserRepository.GetUserByEmailAsync(userTwo.Email);
            Assert.NotNull(retrievedUserTwo);
            Assert.Equal(sharedOriginalHash, retrievedUserTwo.PasswordHash);
        }
    }
}
