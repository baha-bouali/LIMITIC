using LIMTIC.Application.Contracts.Commands.ResearchAxis;
using LIMTIC.Application.Contracts.Commands.UpdateUserRole;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
using LIMTIC.UnitTests.Base;

namespace LIMTIC.UnitTests.Tests
{
    public class ResearchAxisTests : BaseTests
    {
        private async Task<Guid> CreateAxisAsync(string title = "AI Research", string description = "Artificial Intelligence studies")
        {
            var result = await ResearchAxisService.CreateAsync(new CreateResearchAxisCommand
            {
                Title = title,
                Description = description,
                Themes = ["Machine Learning", "Deep Learning"]
            });
            Assert.True(result.Success);
            return result.Data!.Id;
        }

        private async Task<Guid> CreateResearcherAsync(string email)
        {
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Researcher",
                LastName = "Unit",
                Email = email,
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
                Role = UserRole.Researcher,
                Rank = "Professor",
                Specialty = "AI",
                Office = "Room 1",
                PhoneNumber = "12345"
            });
            Assert.True(result.Success);
            return user.Id;
        }

        // ── CREATE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateResearchAxis_ValidData_ReturnsDto()
        {
            var result = await ResearchAxisService.CreateAsync(new CreateResearchAxisCommand
            {
                Title = "Cybersecurity",
                Description = "Security research",
                Themes = ["Cryptography", "Network Security"]
            });

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Cybersecurity", result.Data!.Title);
            Assert.Equal("Security research", result.Data.Description);
            Assert.Equal(2, result.Data.Themes.Length);
        }

        [Fact]
        public async Task CreateResearchAxis_MissingTitle_ReturnsValidationFailure()
        {
            var result = await ResearchAxisService.CreateAsync(new CreateResearchAxisCommand
            {
                Title = "",
                Description = "Some description"
            });

            Assert.False(result.Success);
            Assert.NotNull(result.ValidationErrors);
        }

        [Fact]
        public async Task CreateResearchAxis_MissingDescription_ReturnsValidationFailure()
        {
            var result = await ResearchAxisService.CreateAsync(new CreateResearchAxisCommand
            {
                Title = "Some Title",
                Description = ""
            });

            Assert.False(result.Success);
            Assert.NotNull(result.ValidationErrors);
        }

        // ── GET ────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_ReturnsAllCreatedAxes()
        {
            await CreateAxisAsync("Axis A", "Desc A");
            await CreateAxisAsync("Axis B", "Desc B");

            var result = await ResearchAxisService.GetAllAsync();

            Assert.True(result.Success);
            Assert.True(result.Data!.Count >= 2);
        }

        [Fact]
        public async Task GetById_ExistingAxis_ReturnsDto()
        {
            var id = await CreateAxisAsync("IoT Security", "Internet of Things research");

            var result = await ResearchAxisService.GetByIdAsync(id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("IoT Security", result.Data!.Title);
        }

        [Fact]
        public async Task GetById_NonExistingAxis_ReturnsFailure()
        {
            var result = await ResearchAxisService.GetByIdAsync(Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Equal("Research axis not found", result.Message);
        }

        // ── UPDATE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateResearchAxis_ValidData_UpdatesFields()
        {
            var id = await CreateAxisAsync("Old Title", "Old Description");

            var result = await ResearchAxisService.UpdateAsync(new UpdateResearchAxisCommand
            {
                Id = id,
                Title = "New Title",
                Description = "New Description",
                Themes = ["Theme X"]
            });

            Assert.True(result.Success);
            Assert.Equal("New Title", result.Data!.Title);
            Assert.Equal("New Description", result.Data.Description);

            var persisted = await ResearchAxisRepository.GetByIdAsync(id);
            Assert.Equal("New Title", persisted!.Title);
        }

        [Fact]
        public async Task UpdateResearchAxis_NonExistingAxis_ReturnsFailure()
        {
            var result = await ResearchAxisService.UpdateAsync(new UpdateResearchAxisCommand
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Description"
            });

            Assert.False(result.Success);
            Assert.Equal("Research axis not found", result.Message);
        }

        [Fact]
        public async Task UpdateResearchAxis_MissingTitle_ReturnsValidationFailure()
        {
            var id = await CreateAxisAsync();

            var result = await ResearchAxisService.UpdateAsync(new UpdateResearchAxisCommand
            {
                Id = id,
                Title = "",
                Description = "Description"
            });

            Assert.False(result.Success);
            Assert.NotNull(result.ValidationErrors);
        }

        // ── DELETE ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteResearchAxis_ExistingAxis_DeletesIt()
        {
            var id = await CreateAxisAsync("To Delete", "Will be removed");

            var result = await ResearchAxisService.DeleteAsync(id);

            Assert.True(result.Success);
            Assert.Null(await ResearchAxisRepository.GetByIdAsync(id));
        }

        [Fact]
        public async Task DeleteResearchAxis_NonExistingAxis_ReturnsFailure()
        {
            var result = await ResearchAxisService.DeleteAsync(Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Equal("Research axis not found", result.Message);
        }

        // ── COLOR & RESPONSIBLE ────────────────────────────────────────────────────

        [Fact]
        public async Task CreateResearchAxis_WithColor_PersistsColor()
        {
            var result = await ResearchAxisService.CreateAsync(new CreateResearchAxisCommand
            {
                Title = "Color Axis",
                Description = "Axis with color",
                Color = "#FF5733"
            });

            Assert.True(result.Success);
            Assert.Equal("#FF5733", result.Data!.Color);
        }

        [Fact]
        public async Task CreateResearchAxis_WithValidResponsible_PersistsResponsible()
        {
            var researcherId = await CreateResearcherAsync("responsible.unit@test.com");

            var result = await ResearchAxisService.CreateAsync(new CreateResearchAxisCommand
            {
                Title = "Axis With Responsible",
                Description = "Has a responsible researcher",
                ResponsibleId = researcherId
            });

            Assert.True(result.Success);
            Assert.Equal(researcherId, result.Data!.ResponsibleId);
        }

        [Fact]
        public async Task CreateResearchAxis_WithNonResearcherResponsible_ReturnsFailure()
        {
            var result = await ResearchAxisService.CreateAsync(new CreateResearchAxisCommand
            {
                Title = "Bad Responsible",
                Description = "Should fail",
                ResponsibleId = Guid.NewGuid()
            });

            Assert.False(result.Success);
            Assert.Equal("Responsible user is not a researcher", result.Message);
        }

        [Fact]
        public async Task CreateResearchAxis_WithMemberIds_PopulatesMembers()
        {
            var researcherId = await CreateResearcherAsync("member.init.unit@test.com");

            var result = await ResearchAxisService.CreateAsync(new CreateResearchAxisCommand
            {
                Title = "Axis With Members",
                Description = "Has initial members",
                MemberIds = [researcherId]
            });

            Assert.True(result.Success);
            Assert.Single(result.Data!.Members);
            Assert.Equal(researcherId, result.Data.Members[0].Id);
        }

        [Fact]
        public async Task CreateResearchAxis_WithNonResearcherMemberId_ReturnsFailure()
        {
            var result = await ResearchAxisService.CreateAsync(new CreateResearchAxisCommand
            {
                Title = "Bad Member",
                Description = "Should fail",
                MemberIds = [Guid.NewGuid()]
            });

            Assert.False(result.Success);
        }

        [Fact]
        public async Task UpdateResearchAxis_WithColor_UpdatesColor()
        {
            var id = await CreateAxisAsync("Color Update Test", "Will get a color");

            var result = await ResearchAxisService.UpdateAsync(new UpdateResearchAxisCommand
            {
                Id = id,
                Title = "Color Update Test",
                Description = "Now has color",
                Color = "#0000FF"
            });

            Assert.True(result.Success);
            Assert.Equal("#0000FF", result.Data!.Color);
        }

        // ── MEMBER MANAGEMENT ─────────────────────────────────────────────────────

        [Fact]
        public async Task AddMember_ValidResearcher_AddsToAxis()
        {
            var axisId = await CreateAxisAsync("Member Add Test", "Axis for adding members");
            var researcherId = await CreateResearcherAsync("addmember.unit@test.com");

            var result = await ResearchAxisService.AddMemberAsync(axisId, researcherId);

            Assert.True(result.Success);

            var axis = await ResearchAxisService.GetByIdAsync(axisId);
            Assert.Contains(axis.Data!.Members, m => m.Id == researcherId);
        }

        [Fact]
        public async Task AddMember_NonResearcher_ReturnsFailure()
        {
            var axisId = await CreateAxisAsync("Non Researcher Member", "Should reject non-researcher");

            var result = await ResearchAxisService.AddMemberAsync(axisId, Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Equal("User is not a researcher", result.Message);
        }

        [Fact]
        public async Task AddMember_AxisNotFound_ReturnsFailure()
        {
            var researcherId = await CreateResearcherAsync("orphan.researcher.unit@test.com");

            var result = await ResearchAxisService.AddMemberAsync(Guid.NewGuid(), researcherId);

            Assert.False(result.Success);
            Assert.Equal("Research axis not found", result.Message);
        }

        [Fact]
        public async Task RemoveMember_ExistingMember_RemovesFromAxis()
        {
            var axisId = await CreateAxisAsync("Remove Member Test", "Axis for removing members");
            var researcherId = await CreateResearcherAsync("removemember.unit@test.com");

            await ResearchAxisService.AddMemberAsync(axisId, researcherId);

            var result = await ResearchAxisService.RemoveMemberAsync(axisId, researcherId);

            Assert.True(result.Success);

            var axis = await ResearchAxisService.GetByIdAsync(axisId);
            Assert.DoesNotContain(axis.Data!.Members, m => m.Id == researcherId);
        }

        [Fact]
        public async Task RemoveMember_NonExistingMember_ReturnsFailure()
        {
            var axisId = await CreateAxisAsync("Remove Ghost Member", "Axis with no members");

            var result = await ResearchAxisService.RemoveMemberAsync(axisId, Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Equal("Member not found in this axis", result.Message);
        }

        [Fact]
        public async Task AddMember_Idempotent_DoesNotDuplicate()
        {
            var axisId = await CreateAxisAsync("Idempotent Add Test", "Axis for duplicate add test");
            var researcherId = await CreateResearcherAsync("idempotent.member.unit@test.com");

            await ResearchAxisService.AddMemberAsync(axisId, researcherId);
            var result = await ResearchAxisService.AddMemberAsync(axisId, researcherId);

            Assert.True(result.Success);

            var axis = await ResearchAxisService.GetByIdAsync(axisId);
            Assert.Equal(1, axis.Data!.Members.Count(m => m.Id == researcherId));
        }
    }
}
