using LIMTIC.Application.Contracts.Commands.UpdateUserRole;
using LIMTIC.Application.Abstractions.UserManagement;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
using LIMTIC.UnitTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace LIMTIC.UnitTests.Tests
{
    public class UpdateUserRoleTests : BaseTests
    {
        [Fact]
        public async Task UpdateUserRole_ToResearcher_CreatesResearcherProfile()
        {
            // Arrange - create a user
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Research",
                LastName = "User",
                Email = "research.user@example.com",
                PasswordHash = "hash",
                Role = UserRole.Visitor,
                IsActive = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            var added = await UserRepository.AddUserAsync(user);
            Assert.True(added);

            var cmd = new UpdateUserRoleCommand
            {
                UserId = user.Id,
                Role = UserRole.Researcher,
                Rank = "Professor",
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345",
                ResearchAxisIds = new List<Guid>()
            };

            // Act
            var result = await UsersManagementService.UpdateUserRoleAsync(cmd);

            // Assert
            Assert.True(result.Success);

            var researcher = await ResearcherRepository.GetByUserIdAsync(user.Id);
            Assert.NotNull(researcher);
            Assert.Equal("Professor", researcher!.Rank);

            var updatedUser = await UserRepository.GetUserByIdAsync(user.Id);
            Assert.Equal(UserRole.Researcher, updatedUser!.Role);
        }

        [Fact]
        public async Task UpdateUserRole_ToPhDStudent_CreatesPhDProfile()
        {
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Phd",
                LastName = "Student",
                Email = "phd.user@example.com",
                PasswordHash = "hash",
                Role = UserRole.Visitor,
                IsActive = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            var added = await UserRepository.AddUserAsync(user);
            Assert.True(added);

            var cmd = new UpdateUserRoleCommand
            {
                UserId = user.Id,
                Role = UserRole.PhDStudent,
                EnrollmentYear = 2023
            };

            var result = await UsersManagementService.UpdateUserRoleAsync(cmd);
            Assert.True(result.Success);

            var phd = await PhDStudentRepository.GetByUserIdAsync(user.Id);
            Assert.NotNull(phd);
            Assert.Equal(2023, phd!.EnrollmentYear);

            var updatedUser = await UserRepository.GetUserByIdAsync(user.Id);
            Assert.Equal(UserRole.PhDStudent, updatedUser!.Role);
        }

        [Fact]
        public async Task UpdateUserRole_ToMasterian_CreatesMasterianProfile()
        {
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Master",
                LastName = "Student",
                Email = "master.user@example.com",
                PasswordHash = "hash",
                Role = UserRole.Visitor,
                IsActive = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            var added = await UserRepository.AddUserAsync(user);
            Assert.True(added);

            var cmd = new UpdateUserRoleCommand
            {
                UserId = user.Id,
                Role = UserRole.Masterian,
                Cohort = "2024",
                DissertationSubject = "Subject X"
            };

            var result = await UsersManagementService.UpdateUserRoleAsync(cmd);
            Assert.True(result.Success);

            var master = await MasterianRepository.GetByUserIdAsync(user.Id);
            Assert.NotNull(master);
            Assert.Equal("2024", master!.Cohort);

            var updatedUser = await UserRepository.GetUserByIdAsync(user.Id);
            Assert.Equal(UserRole.Masterian, updatedUser!.Role);
        }
    }
}
