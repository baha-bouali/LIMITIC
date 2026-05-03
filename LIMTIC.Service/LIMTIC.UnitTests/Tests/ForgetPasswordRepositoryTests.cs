using LIMTIC.Domain.Entities;
using LIMTIC.Domain.Enums;
using LIMTIC.Infrastructure.Repositories;
using LIMTIC.UnitTests.Base;

namespace LIMTIC.UnitTests.Tests
{
    public class ForgetPasswordRepositoryTests : BaseTests
    {
        private static User BuildUser(string email) => new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Doe",
            Email = email,
            PasswordHash = "hashed_password",
            Role = UserRole.Admin,
            AvatarBlobName = null,
            IsActive = true,
            CreatedBy = Guid.NewGuid(),
            CreatedAtUtc = DateTime.UtcNow,
        };

        // ── Forget Password ───────────────────────────────────────────────────

        [Fact]
        public async Task ForgetPassword_AddOTPToken_TokenIsPersistedForUser()
        {
            // Steps:
            // 1. Create and add a user
            // 2. Add a reset password entry with an OTP token
            // 3. Retrieve the token and assert it matches

            // 1. Create and add a user
            var user = BuildUser("forget.addotp@example.com");
            Assert.True(await UserRepository.AddUserAsync(user));

            // 2. Add a reset password entry with an OTP token
            var resetPassword = new ResetPassword
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                OTPTokenHash = "hashed_otp_token",
                OTPTokenExpiry = DateTime.UtcNow.AddMinutes(10),
            };
            await ResetPasswordRepository.AddOTPTokenAsync(resetPassword);

            // 3. Retrieve the token and assert it matches
            var retrieved = await ResetPasswordRepository.GetTokenAsync(user.Id);
            Assert.NotNull(retrieved);
            Assert.Equal(user.Id, retrieved.UserId);
            Assert.Equal("hashed_otp_token", retrieved.OTPTokenHash);
            Assert.NotNull(retrieved.OTPTokenExpiry);
        }

        [Fact]
        public async Task ForgetPassword_UpdateExistingOTPToken_NewTokenReplacesPrevious()
        {
            // Steps:
            // 1. Create and add a user
            // 2. Add an initial OTP token
            // 3. Update the entry with a new OTP token
            // 4. Retrieve and assert the new token is stored and reset token fields are cleared

            // 1. Create and add a user
            var user = BuildUser("forget.updateotp@example.com");
            Assert.True(await UserRepository.AddUserAsync(user));

            // 2. Add an initial OTP token
            var resetPassword = new ResetPassword
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                OTPTokenHash = "hashed_otp_old",
                OTPTokenExpiry = DateTime.UtcNow.AddMinutes(10),
            };
            await ResetPasswordRepository.AddOTPTokenAsync(resetPassword);

            // 3. Update the entry with a new OTP token and clear reset token fields
            var existing = await ResetPasswordRepository.GetTokenAsync(user.Id);
            Assert.NotNull(existing);
            existing.OTPTokenHash = "hashed_otp_new";
            existing.OTPTokenExpiry = DateTime.UtcNow.AddMinutes(10);
            existing.ResetPasswordTokenHash = null;
            existing.ResetPasswordTokenExpiry = null;
            await ResetPasswordRepository.UpdateResetPasswordTokenAsync(existing);

            // 4. Retrieve and assert the new token is stored and reset token fields are cleared
            var retrieved = await ResetPasswordRepository.GetTokenAsync(user.Id);
            Assert.NotNull(retrieved);
            Assert.Equal("hashed_otp_new", retrieved.OTPTokenHash);
            Assert.Null(retrieved.ResetPasswordTokenHash);
            Assert.Null(retrieved.ResetPasswordTokenExpiry);
        }

        // ── Verify OTP ────────────────────────────────────────────────────────

        [Fact]
        public async Task VerifyOTP_UpdateResetPasswordToken_TokenIsPersistedAfterVerification()
        {
            // Steps:
            // 1. Create and add a user
            // 2. Add an OTP token entry
            // 3. Simulate verification by updating with a reset password token
            // 4. Retrieve and assert the reset password token is stored

            // 1. Create and add a user
            var user = BuildUser("verify.otp@example.com");
            Assert.True(await UserRepository.AddUserAsync(user));

            // 2. Add an OTP token entry
            var resetPassword = new ResetPassword
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                OTPTokenHash = "hashed_otp_token",
                OTPTokenExpiry = DateTime.UtcNow.AddMinutes(10),
            };
            await ResetPasswordRepository.AddOTPTokenAsync(resetPassword);

            // 3. Simulate verification by updating with a reset password token
            var existing = await ResetPasswordRepository.GetTokenAsync(user.Id);
            Assert.NotNull(existing);
            existing.ResetPasswordTokenHash = "hashed_reset_token";
            existing.ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(15);
            await ResetPasswordRepository.UpdateResetPasswordTokenAsync(existing);

            // 4. Retrieve and assert the reset password token is stored
            var retrieved = await ResetPasswordRepository.GetTokenAsync(user.Id);
            Assert.NotNull(retrieved);
            Assert.Equal("hashed_reset_token", retrieved.ResetPasswordTokenHash);
            Assert.NotNull(retrieved.ResetPasswordTokenExpiry);
        }

        [Fact]
        public async Task VerifyOTP_GetTokenAsync_ReturnsNullForUserWithNoEntry()
        {
            // Steps:
            // 1. Create and add a user without adding any reset password entry
            // 2. Assert that retrieving the token returns null

            // 1. Create and add a user without adding any reset password entry
            var user = BuildUser("verify.noentry@example.com");
            Assert.True(await UserRepository.AddUserAsync(user));

            // 2. Assert that retrieving the token returns null
            var retrieved = await ResetPasswordRepository.GetTokenAsync(user.Id);
            Assert.Null(retrieved);
        }

        // ── Reset Password ────────────────────────────────────────────────────

        [Fact]
        public async Task ResetPassword_UpdateUserPassword_PasswordHashIsUpdated()
        {
            // Steps:
            // 1. Create and add a user with a known password hash
            // 2. Add a reset password entry with a valid reset token
            // 3. Update the user's password via the repository
            // 4. Retrieve the user and assert the password hash was updated

            // 1. Create and add a user with a known password hash
            var user = BuildUser("reset.password@example.com");
            Assert.True(await UserRepository.AddUserAsync(user));

            // 2. Add a reset password entry with a valid reset token
            var resetPassword = new ResetPassword
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                OTPTokenHash = "hashed_otp_token",
                OTPTokenExpiry = DateTime.UtcNow.AddMinutes(10),
                ResetPasswordTokenHash = "hashed_reset_token",
                ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(15),
            };
            await ResetPasswordRepository.AddOTPTokenAsync(resetPassword);

            // 3. Update the user's password via the repository
            const string newHash = "new_hashed_password";
            var result = await UserRepository.UpdateUserPasswordAsync(user, newHash);
            Assert.True(result);

            // 4. Retrieve the user and assert the password hash was updated
            var retrievedUser = await UserRepository.GetUserByEmailAsync(user.Email);
            Assert.NotNull(retrievedUser);
            Assert.Equal(newHash, retrievedUser.PasswordHash);
        }

        [Fact]
        public async Task ResetPassword_DoesNotAffectOtherUserPassword()
        {
            // Steps:
            // 1. Create and add two users
            // 2. Reset the password for the first user only
            // 3. Assert the second user's password hash is unchanged

            // 1. Create and add two users
            const string originalHash = "original_hashed_password";
            var userOne = BuildUser("reset.userone@example.com");
            var userTwo = BuildUser("reset.usertwo@example.com");
            userTwo.PasswordHash = originalHash;

            Assert.True(await UserRepository.AddUserAsync(userOne));
            Assert.True(await UserRepository.AddUserAsync(userTwo));

            // 2. Reset the password for the first user only
            var result = await UserRepository.UpdateUserPasswordAsync(userOne, "userone_new_hash");
            Assert.True(result);

            // 3. Assert the second user's password hash is unchanged
            var retrievedUserTwo = await UserRepository.GetUserByEmailAsync(userTwo.Email);
            Assert.NotNull(retrievedUserTwo);
            Assert.Equal(originalHash, retrievedUserTwo.PasswordHash);
        }
    }
}