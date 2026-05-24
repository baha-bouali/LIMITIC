using LIMTIC.Application.Contracts.Commands.Profiles;
using LIMTIC.Application.Contracts.Commands.UpdateUserRole;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
using LIMTIC.UnitTests.Base;

namespace LIMTIC.UnitTests.Tests
{
    public class PhDStudentProfileTests : BaseTests
    {
        private static UserEntity BuildVisitorUser(string email) => new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "PhDStudent",
            Email = email,
            PasswordHash = "hash",
            Role = UserRole.Visitor,
            IsActive = true,
            CreatedBy = Guid.NewGuid(),
            CreatedAtUtc = DateTime.UtcNow
        };

        private async Task<UserEntity> CreatePhDStudentUser(string email, int enrollmentYear = 2022)
        {
            var user = BuildVisitorUser(email);
            await UserRepository.AddUserAsync(user);
            await UsersManagementService.UpdateUserRoleAsync(new UpdateUserRoleCommand
            {
                UserId = user.Id,
                Role = UserRole.PhDStudent,
                EnrollmentYear = enrollmentYear
            });
            return user;
        }

        [Fact]
        public async Task GetPhDStudentProfile_ExistingProfile_ReturnsCombinedDto()
        {
            // Steps:
            // 1. Create a PhDStudent user
            // 2. Call GetByUserIdAsync
            // 3. Assert DTO contains both user and PhDStudent fields

            var user = await CreatePhDStudentUser("get.phd.profile@example.com", 2021);

            var result = await PhDStudentProfileService.GetByUserIdAsync(user.Id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(user.Id, result.Data!.Id);
            Assert.Equal(user.Email, result.Data.Email);
            Assert.Equal(2021, result.Data.EnrollmentYear);
            Assert.Equal(UserRole.PhDStudent, result.Data.Role);
        }

        [Fact]
        public async Task GetPhDStudentProfile_NonExistingProfile_ReturnsFailure()
        {
            var result = await PhDStudentProfileService.GetByUserIdAsync(Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Equal("PhD student profile not found", result.Message);
        }

        [Fact]
        public async Task UpdatePhDStudentProfile_ValidData_UpdatesFields()
        {
            // Steps:
            // 1. Create a PhDStudent user
            // 2. Call UpdateAsync with new values
            // 3. Assert DTO and persisted entity reflect changes

            var user = await CreatePhDStudentUser("update.phd.profile@example.com", 2022);

            var command = new UpdatePhDStudentProfileCommand
            {
                UserId = user.Id,
                EnrollmentYear = 2023,
                ThesisSubject = "Federated Learning",
                PhotoUrl = "https://photo.test/phd.jpg"
            };

            var result = await PhDStudentProfileService.UpdateAsync(command);

            Assert.True(result.Success);
            Assert.Equal(2023, result.Data!.Profile.EnrollmentYear);
            Assert.Equal("Federated Learning", result.Data.Profile.ThesisSubject);

            var persisted = await PhDStudentRepository.GetByUserIdAsync(user.Id);
            Assert.Equal(2023, persisted!.EnrollmentYear);
            Assert.Equal("Federated Learning", persisted.ThesisSubject);
        }

        [Fact]
        public async Task UpdatePhDStudentProfile_InvalidEnrollmentYear_ReturnsValidationFailure()
        {
            var user = await CreatePhDStudentUser("update.phd.invalid@example.com");

            var command = new UpdatePhDStudentProfileCommand
            {
                UserId = user.Id,
                EnrollmentYear = 0   // must be > 0
            };

            var result = await PhDStudentProfileService.UpdateAsync(command);

            Assert.False(result.Success);
            Assert.NotNull(result.ValidationErrors);
        }

        [Fact]
        public async Task UpdatePhDStudentProfile_NonExistingProfile_ReturnsFailure()
        {
            var command = new UpdatePhDStudentProfileCommand
            {
                UserId = Guid.NewGuid(),
                EnrollmentYear = 2022
            };

            var result = await PhDStudentProfileService.UpdateAsync(command);

            Assert.False(result.Success);
            Assert.Equal("PhD student profile not found", result.Message);
        }

        [Fact]
        public async Task DeletePhDStudentProfile_ExistingProfile_DeletesAndResetsRoleToVisitor()
        {
            // Steps:
            // 1. Create a PhDStudent user
            // 2. Call DeleteAsync
            // 3. Assert profile row is gone and user role is reset to Visitor

            var user = await CreatePhDStudentUser("delete.phd.profile@example.com");

            var result = await PhDStudentProfileService.DeleteAsync(user.Id);

            Assert.True(result.Success);

            Assert.Null(await PhDStudentRepository.GetByUserIdAsync(user.Id));

            var updatedUser = await UserRepository.GetUserByIdAsync(user.Id);
            Assert.Equal(UserRole.Visitor, updatedUser!.Role);
        }

        [Fact]
        public async Task DeletePhDStudentProfile_NonExistingProfile_ReturnsFailure()
        {
            var result = await PhDStudentProfileService.DeleteAsync(Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Equal("PhD student profile not found", result.Message);
        }
    }
}
