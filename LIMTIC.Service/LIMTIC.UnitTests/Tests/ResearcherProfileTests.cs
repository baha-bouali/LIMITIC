using LIMTIC.Application.Contracts.Commands.Profiles;
using LIMTIC.Application.Contracts.Commands.UpdateUserRole;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
using LIMTIC.UnitTests.Base;

namespace LIMTIC.UnitTests.Tests
{
    public class ResearcherProfileTests : BaseTests
    {
        private static UserEntity BuildVisitorUser(string email) => new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Researcher",
            Email = email,
            PasswordHash = "hash",
            Role = UserRole.Visitor,
            IsActive = true,
            CreatedBy = Guid.NewGuid(),
            CreatedAtUtc = DateTime.UtcNow
        };

        private async Task<UserEntity> CreateResearcherUser(string email)
        {
            var user = BuildVisitorUser(email);
            await UserRepository.AddUserAsync(user);
            await UsersManagementService.UpdateUserRoleAsync(new UpdateUserRoleCommand
            {
                UserId = user.Id,
                Role = UserRole.Researcher,
                Rank = "Professor",
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345"
            });
            return user;
        }

        [Fact]
        public async Task GetResearcherProfile_ExistingProfile_ReturnsCombinedDto()
        {
            // Steps:
            // 1. Create a user and promote to Researcher
            // 2. Call GetByUserIdAsync
            // 3. Assert profile DTO contains both user and researcher fields

            var user = await CreateResearcherUser("get.researcher.profile@example.com");

            var result = await ResearcherProfileService.GetByUserIdAsync(user.Id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(user.Id, result.Data!.Id);
            Assert.Equal(user.Email, result.Data.Email);
            Assert.Equal("Professor", result.Data.Rank);
            Assert.Equal("AI", result.Data.Specialty);
            Assert.Equal(UserRole.Researcher, result.Data.Role);
        }

        [Fact]
        public async Task GetResearcherProfile_NonExistingProfile_ReturnsFailure()
        {
            var result = await ResearcherProfileService.GetByUserIdAsync(Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Equal("Researcher profile not found", result.Message);
        }

        [Fact]
        public async Task UpdateResearcherProfile_ValidData_UpdatesAllFields()
        {
            // Steps:
            // 1. Create a researcher user
            // 2. Call UpdateAsync with new field values
            // 3. Assert the returned DTO and the persisted entity reflect the changes

            var user = await CreateResearcherUser("update.researcher.profile@example.com");

            var command = new UpdateResearcherProfileCommand
            {
                UserId = user.Id,
                Rank = "Full Professor",
                Specialty = "Deep Learning",
                Office = "Room 42",
                PhoneNumber = "99999",
                Biography = "Expert in DL",
                LinkedIn = "https://linkedin.com/test"
            };

            var result = await ResearcherProfileService.UpdateAsync(command);

            Assert.True(result.Success);
            Assert.Equal("Full Professor", result.Data!.Profile.Rank);
            Assert.Equal("Deep Learning", result.Data.Profile.Specialty);
            Assert.Equal("Expert in DL", result.Data.Profile.Biography);
            Assert.Equal("https://linkedin.com/test", result.Data.Profile.LinkedIn);

            // Verify persistence
            var persisted = await ResearcherRepository.GetByUserIdAsync(user.Id);
            Assert.Equal("Full Professor", persisted!.Rank);
            Assert.Equal("Room 42", persisted.Office);
        }

        [Fact]
        public async Task UpdateResearcherProfile_MissingRequiredFields_ReturnsValidationFailure()
        {
            var user = await CreateResearcherUser("update.researcher.invalid@example.com");

            var command = new UpdateResearcherProfileCommand
            {
                UserId = user.Id,
                Rank = "",        // required
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345"
            };

            var result = await ResearcherProfileService.UpdateAsync(command);

            Assert.False(result.Success);
            Assert.NotNull(result.ValidationErrors);
        }

        [Fact]
        public async Task UpdateResearcherProfile_NonExistingProfile_ReturnsFailure()
        {
            var command = new UpdateResearcherProfileCommand
            {
                UserId = Guid.NewGuid(),
                Rank = "Prof",
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345"
            };

            var result = await ResearcherProfileService.UpdateAsync(command);

            Assert.False(result.Success);
            Assert.Equal("Researcher profile not found", result.Message);
        }

        [Fact]
        public async Task DeleteResearcherProfile_ExistingProfile_DeletesAndResetsRoleToVisitor()
        {
            // Steps:
            // 1. Create a researcher user
            // 2. Call DeleteAsync
            // 3. Assert profile row is gone and user role is reset to Visitor

            var user = await CreateResearcherUser("delete.researcher.profile@example.com");

            var result = await ResearcherProfileService.DeleteAsync(user.Id);

            Assert.True(result.Success);

            // Profile must be gone
            Assert.Null(await ResearcherRepository.GetByUserIdAsync(user.Id));

            // User role must be reset
            var updatedUser = await UserRepository.GetUserByIdAsync(user.Id);
            Assert.Equal(UserRole.Visitor, updatedUser!.Role);
        }

        [Fact]
        public async Task DeleteResearcherProfile_NonExistingProfile_ReturnsFailure()
        {
            var result = await ResearcherProfileService.DeleteAsync(Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Equal("Researcher profile not found", result.Message);
        }
    }
}
