using LIMTIC.Application.Contracts.Commands.ResearchAxis;
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
    }
}
