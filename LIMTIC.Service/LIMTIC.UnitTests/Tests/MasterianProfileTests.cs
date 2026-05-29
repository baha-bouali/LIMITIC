using LIMTIC.Application.Contracts.Commands.Profiles;
using LIMTIC.Application.Contracts.Commands.UpdateUserRole;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
using LIMTIC.UnitTests.Base;

namespace LIMTIC.UnitTests.Tests
{
    public class MasterianProfileTests : BaseTests
    {
        private static UserEntity BuildVisitorUser(string email) => new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Masterian",
            Email = email,
            PasswordHash = "hash",
            Role = UserRole.Visitor,
            IsActive = true,
            CreatedBy = Guid.NewGuid(),
            CreatedAtUtc = DateTime.UtcNow
        };

        private async Task<UserEntity> CreateMasterianUser(string email, string cohort = "2024", string dissertation = "Initial Subject")
        {
            var user = BuildVisitorUser(email);
            await UserRepository.AddUserAsync(user);
            await UsersManagementService.UpdateUserRoleAsync(new UpdateUserRoleCommand
            {
                UserId = user.Id,
                Role = UserRole.Masterian,
                Cohort = cohort,
                DissertationSubject = dissertation
            });
            return user;
        }

        [Fact]
        public async Task GetMasterianProfile_ExistingProfile_ReturnsCombinedDto()
        {
            // Steps:
            // 1. Create a Masterian user
            // 2. Call GetByUserIdAsync
            // 3. Assert DTO contains both user and Masterian fields

            var user = await CreateMasterianUser("get.masterian.profile@example.com", "2023", "Blockchain Security");

            var result = await MasterianProfileService.GetByUserIdAsync(user.Id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(user.Id, result.Data!.Id);
            Assert.Equal(user.Email, result.Data.Email);
            Assert.Equal("2023", result.Data.Cohort);
            Assert.Equal("Blockchain Security", result.Data.DissertationSubject);
            Assert.Equal(UserRole.Masterian, result.Data.Role);
        }

        [Fact]
        public async Task GetMasterianProfile_NonExistingProfile_ReturnsFailure()
        {
            var result = await MasterianProfileService.GetByUserIdAsync(Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Equal("Masterian profile not found", result.Message);
        }

        [Fact]
        public async Task UpdateMasterianProfile_ValidData_UpdatesFields()
        {
            // Steps:
            // 1. Create a Masterian user
            // 2. Call UpdateAsync with new values
            // 3. Assert DTO and persisted entity reflect changes

            var user = await CreateMasterianUser("update.masterian.profile@example.com");

            var command = new UpdateMasterianProfileCommand
            {
                UserId = user.Id,
                Cohort = "2025",
                DissertationSubject = "Edge Computing Security"
            };

            var result = await MasterianProfileService.UpdateAsync(command);

            Assert.True(result.Success);
            Assert.Equal("2025", result.Data!.Cohort);
            Assert.Equal("Edge Computing Security", result.Data.DissertationSubject);

            var persisted = await MasterianRepository.GetByUserIdAsync(user.Id);
            Assert.Equal("2025", persisted!.Cohort);
            Assert.Equal("Edge Computing Security", persisted.DissertationSubject);
        }

        [Fact]
        public async Task UpdateMasterianProfile_MissingRequiredFields_ReturnsValidationFailure()
        {
            var user = await CreateMasterianUser("update.masterian.invalid@example.com");

            var command = new UpdateMasterianProfileCommand
            {
                UserId = user.Id,
                Cohort = "",                  // required
                DissertationSubject = ""      // required
            };

            var result = await MasterianProfileService.UpdateAsync(command);

            Assert.False(result.Success);
            Assert.NotNull(result.ValidationErrors);
        }

        [Fact]
        public async Task UpdateMasterianProfile_NonExistingProfile_ReturnsFailure()
        {
            var command = new UpdateMasterianProfileCommand
            {
                UserId = Guid.NewGuid(),
                Cohort = "2024",
                DissertationSubject = "Something"
            };

            var result = await MasterianProfileService.UpdateAsync(command);

            Assert.False(result.Success);
            Assert.Equal("Masterian profile not found", result.Message);
        }

        [Fact]
        public async Task DeleteMasterianProfile_ExistingProfile_DeletesAndResetsRoleToVisitor()
        {
            // Steps:
            // 1. Create a Masterian user
            // 2. Call DeleteAsync
            // 3. Assert profile row is gone and user role is reset to Visitor

            var user = await CreateMasterianUser("delete.masterian.profile@example.com");

            var result = await MasterianProfileService.DeleteAsync(user.Id);

            Assert.True(result.Success);

            Assert.Null(await MasterianRepository.GetByUserIdAsync(user.Id));

            var updatedUser = await UserRepository.GetUserByIdAsync(user.Id);
            Assert.Equal(UserRole.Visitor, updatedUser!.Role);
        }

        [Fact]
        public async Task DeleteMasterianProfile_NonExistingProfile_ReturnsFailure()
        {
            var result = await MasterianProfileService.DeleteAsync(Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Equal("Masterian profile not found", result.Message);
        }
    }
}
