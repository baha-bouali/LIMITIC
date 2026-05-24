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

        [Fact]
        public async Task UpdateUserRole_ResearcherToMasterian_DeletesResearcherAndCreatesMasterian()
        {
            // Steps:
            // 1. Create a Visitor user and promote to Researcher
            // 2. Then promote to Masterian
            // 3. Assert the Researcher row is gone and Masterian row exists

            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Switch",
                LastName = "User",
                Email = "switch.researcher.to.master@example.com",
                PasswordHash = "hash",
                Role = UserRole.Visitor,
                IsActive = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };
            await UserRepository.AddUserAsync(user);

            // Promote to Researcher
            var toResearcher = new UpdateUserRoleCommand
            {
                UserId = user.Id,
                Role = UserRole.Researcher,
                Rank = "Associate",
                Specialty = "ML",
                Office = "Lab A",
                PhoneNumber = "99999"
            };
            var r1 = await UsersManagementService.UpdateUserRoleAsync(toResearcher);
            Assert.True(r1.Success);
            Assert.NotNull(await ResearcherRepository.GetByUserIdAsync(user.Id));

            // Now promote to Masterian
            var toMasterian = new UpdateUserRoleCommand
            {
                UserId = user.Id,
                Role = UserRole.Masterian,
                Cohort = "2025",
                DissertationSubject = "Deep Learning"
            };
            var r2 = await UsersManagementService.UpdateUserRoleAsync(toMasterian);
            Assert.True(r2.Success);

            // Researcher row must be deleted
            Assert.Null(await ResearcherRepository.GetByUserIdAsync(user.Id));

            // Masterian row must exist
            var masterian = await MasterianRepository.GetByUserIdAsync(user.Id);
            Assert.NotNull(masterian);
            Assert.Equal("2025", masterian!.Cohort);

            var updatedUser = await UserRepository.GetUserByIdAsync(user.Id);
            Assert.Equal(UserRole.Masterian, updatedUser!.Role);
        }

        [Fact]
        public async Task UpdateUserRole_PhDStudentToResearcher_DeletesPhDAndCreatesResearcher()
        {
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Switch",
                LastName = "PhD",
                Email = "switch.phd.to.researcher@example.com",
                PasswordHash = "hash",
                Role = UserRole.Visitor,
                IsActive = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };
            await UserRepository.AddUserAsync(user);

            // Promote to PhDStudent
            await UsersManagementService.UpdateUserRoleAsync(new UpdateUserRoleCommand
            {
                UserId = user.Id,
                Role = UserRole.PhDStudent,
                EnrollmentYear = 2022
            });
            Assert.NotNull(await PhDStudentRepository.GetByUserIdAsync(user.Id));

            // Now promote to Researcher
            var r = await UsersManagementService.UpdateUserRoleAsync(new UpdateUserRoleCommand
            {
                UserId = user.Id,
                Role = UserRole.Researcher,
                Rank = "Dr",
                Specialty = "NLP",
                Office = "B3",
                PhoneNumber = "11111"
            });
            Assert.True(r.Success);

            Assert.Null(await PhDStudentRepository.GetByUserIdAsync(user.Id));
            Assert.NotNull(await ResearcherRepository.GetByUserIdAsync(user.Id));

            var updatedUser = await UserRepository.GetUserByIdAsync(user.Id);
            Assert.Equal(UserRole.Researcher, updatedUser!.Role);
        }

        [Fact]
        public async Task UpdateUserRole_ResearcherToVisitor_DeletesResearcherProfile()
        {
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Back",
                LastName = "ToVisitor",
                Email = "researcher.back.visitor@example.com",
                PasswordHash = "hash",
                Role = UserRole.Visitor,
                IsActive = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };
            await UserRepository.AddUserAsync(user);

            await UsersManagementService.UpdateUserRoleAsync(new UpdateUserRoleCommand
            {
                UserId = user.Id,
                Role = UserRole.Researcher,
                Rank = "Prof",
                Specialty = "CV",
                Office = "C1",
                PhoneNumber = "22222"
            });
            Assert.NotNull(await ResearcherRepository.GetByUserIdAsync(user.Id));

            var r = await UsersManagementService.UpdateUserRoleAsync(new UpdateUserRoleCommand
            {
                UserId = user.Id,
                Role = UserRole.Visitor
            });
            Assert.True(r.Success);

            Assert.Null(await ResearcherRepository.GetByUserIdAsync(user.Id));

            var updatedUser = await UserRepository.GetUserByIdAsync(user.Id);
            Assert.Equal(UserRole.Visitor, updatedUser!.Role);
        }

        [Fact]
        public async Task UpdateUserRole_UserNotFound_ReturnsFailure()
        {
            var cmd = new UpdateUserRoleCommand
            {
                UserId = Guid.NewGuid(),
                Role = UserRole.Admin
            };

            var result = await UsersManagementService.UpdateUserRoleAsync(cmd);

            Assert.False(result.Success);
            Assert.Equal("User not found", result.Message);
        }

        [Fact]
        public async Task UpdateUserRole_ToResearcher_MissingRequiredFields_ReturnsFailure()
        {
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Bad",
                LastName = "Fields",
                Email = "bad.researcher.fields@example.com",
                PasswordHash = "hash",
                Role = UserRole.Visitor,
                IsActive = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };
            await UserRepository.AddUserAsync(user);

            // Missing Rank, Specialty, Office, PhoneNumber
            var result = await UsersManagementService.UpdateUserRoleAsync(new UpdateUserRoleCommand
            {
                UserId = user.Id,
                Role = UserRole.Researcher
            });

            Assert.False(result.Success);
        }

        [Fact]
        public async Task UpdateUserRole_ToPhDStudent_MissingEnrollmentYear_ReturnsFailure()
        {
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Bad",
                LastName = "Phd",
                Email = "bad.phd.fields@example.com",
                PasswordHash = "hash",
                Role = UserRole.Visitor,
                IsActive = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };
            await UserRepository.AddUserAsync(user);

            var result = await UsersManagementService.UpdateUserRoleAsync(new UpdateUserRoleCommand
            {
                UserId = user.Id,
                Role = UserRole.PhDStudent
                // EnrollmentYear intentionally missing
            });

            Assert.False(result.Success);
        }

        [Fact]
        public async Task UpdateUserRole_ToMasterian_MissingRequiredFields_ReturnsFailure()
        {
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Bad",
                LastName = "Master",
                Email = "bad.masterian.fields@example.com",
                PasswordHash = "hash",
                Role = UserRole.Visitor,
                IsActive = true,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };
            await UserRepository.AddUserAsync(user);

            var result = await UsersManagementService.UpdateUserRoleAsync(new UpdateUserRoleCommand
            {
                UserId = user.Id,
                Role = UserRole.Masterian
                // Cohort and DissertationSubject intentionally missing
            });

            Assert.False(result.Success);
        }
    }
}
