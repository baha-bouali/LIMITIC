using LIMTIC.Domain.Entities.Events;
using LIMTIC.Domain.Entities.ResearchAxis;
using LIMTIC.Domain.Enums;
using LIMTIC.UnitTests.Base;

namespace LIMTIC.UnitTests.Tests
{
    public class EventsRepositoryTests : BaseTests
    {
        [Fact]
        public async Task GetEventsAsyncReturnsFilteredAndPaginatedEvents()
        {
            // Steps:
            // 1. Create a research axis
            // 2. Create multiple events with different statuses and types
            // 3. Retrieve events with pagination and filters
            // 4. Assert that the correct events are returned with proper pagination

            // 1. Create a research axis
            var researchAxis = new ResearchAxisEntity
            {
                Id = Guid.NewGuid(),
                Title = "AI Research",
                Description = "Research in Artificial Intelligence",
                Themes = new[] { "Machine Learning", "Deep Learning" },
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };
            DbContext.ResearchAxes.Add(researchAxis);

            // 2. Create multiple events with different statuses and types
            var upcomingEvent = new EventEntity
            {
                Id = Guid.NewGuid(),
                Title = "Future AI Conference",
                Type = EventType.Conference,
                StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddDays(12),
                Location = "Virtual",
                Description = "An upcoming conference",
                ResearchAxisId = researchAxis.Id,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            var ongoingEvent = new EventEntity
            {
                Id = Guid.NewGuid(),
                Title = "Current AI Seminar",
                Type = EventType.Seminar,
                StartDate = DateTime.UtcNow.AddHours(-2),
                EndDate = DateTime.UtcNow.AddHours(2),
                Location = "Room 101",
                Description = "An ongoing seminar",
                ResearchAxisId = researchAxis.Id,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            var pastEvent = new EventEntity
            {
                Id = Guid.NewGuid(),
                Title = "Past ML Workshop",
                Type = EventType.Workshop,
                StartDate = DateTime.UtcNow.AddDays(-5),
                EndDate = DateTime.UtcNow.AddDays(-3),
                Location = "Room 202",
                Description = "A past workshop",
                ResearchAxisId = researchAxis.Id,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            DbContext.Events.AddRange(upcomingEvent, ongoingEvent, pastEvent);
            await DbContext.SaveChangesAsync();

            // 3. Retrieve events with pagination
            var allEvents = await EventsRepository.GetEventsAsync(null, null, 1, 20, null);
            Assert.NotEmpty(allEvents);
            var initialCount = allEvents.Count;

            // 4. Retrieve only upcoming events
            var upcomingEvents = await EventsRepository.GetEventsAsync("upcoming", null, 1, 20, null);
            Assert.NotEmpty(upcomingEvents);
            Assert.True(upcomingEvents.Any(e => e.Id == upcomingEvent.Id));

            // 5. Retrieve only conference type
            var conferences = await EventsRepository.GetEventsAsync(null, "conference", 1, 20, null);
            Assert.NotEmpty(conferences);
            Assert.True(conferences.Any(e => e.Id == upcomingEvent.Id));

            // 6. Search by title containing "Future"
            var searchResults = await EventsRepository.GetEventsAsync(null, null, 1, 20, "Future");
            Assert.NotEmpty(searchResults);
            Assert.True(searchResults.Any(e => e.Title.Contains("Future")));
        }

        [Fact]
        public async Task GetTotalEventsCountAsyncReturnsCorrectCount()
        {
            // Steps:
            // 1. Create a research axis
            // 2. Create multiple events
            // 3. Get total count without filters
            // 4. Get total count with filters
            // 5. Assert counts are correct

            // 1. Create a research axis
            var researchAxis = new ResearchAxisEntity
            {
                Id = Guid.NewGuid(),
                Title = "AI Research",
                Description = "Research in Artificial Intelligence",
                Themes = new[] { "Machine Learning", "Deep Learning" },
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };
            DbContext.ResearchAxes.Add(researchAxis);

            // 2. Create multiple events
            var events = new[]
            {
                new EventEntity
                {
                    Id = Guid.NewGuid(),
                    Title = "Event 1",
                    Type = EventType.Conference,
                    StartDate = DateTime.UtcNow.AddDays(10),
                    EndDate = DateTime.UtcNow.AddDays(12),
                    Location = "Virtual",
                    Description = "Description 1",
                    ResearchAxisId = researchAxis.Id,
                    CreatedBy = Guid.NewGuid(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new EventEntity
                {
                    Id = Guid.NewGuid(),
                    Title = "Event 2",
                    Type = EventType.Seminar,
                    StartDate = DateTime.UtcNow.AddDays(5),
                    EndDate = DateTime.UtcNow.AddDays(6),
                    Location = "Room 101",
                    Description = "Description 2",
                    ResearchAxisId = researchAxis.Id,
                    CreatedBy = Guid.NewGuid(),
                    CreatedAtUtc = DateTime.UtcNow
                }
            };

            DbContext.Events.AddRange(events);
            await DbContext.SaveChangesAsync();

            // 3. Get total count without filters
            var totalCount = await EventsRepository.GetTotalEventsCountAsync(null, null, null);
            Assert.Equal(2, totalCount);

            // 4. Get total count with type filter
            var conferenceCount = await EventsRepository.GetTotalEventsCountAsync(null, "conference", null);
            Assert.Single(new[] { 1 }.Where(x => x == conferenceCount));

            // 5. Get total count with search filter
            var searchCount = await EventsRepository.GetTotalEventsCountAsync(null, null, "Event");
            Assert.Equal(2, searchCount);
        }

        [Fact]
        public async Task GetEventsAsyncRespectsPagination()
        {
            // Steps:
            // 1. Create a research axis
            // 2. Create 5 events
            // 3. Retrieve first page with limit 2
            // 4. Retrieve second page with limit 2
            // 5. Assert correct events on each page

            // 1. Create a research axis
            var researchAxis = new ResearchAxisEntity
            {
                Id = Guid.NewGuid(),
                Title = "AI Research",
                Description = "Research in Artificial Intelligence",
                Themes = new[] { "Machine Learning", "Deep Learning" },
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };
            DbContext.ResearchAxes.Add(researchAxis);

            // 2. Create 5 events
            var eventIds = new List<Guid>();
            for (int i = 0; i < 5; i++)
            {
                var eventId = Guid.NewGuid();
                eventIds.Add(eventId);
                var evt = new EventEntity
                {
                    Id = eventId,
                    Title = $"Event {i + 1}",
                    Type = EventType.Conference,
                    StartDate = DateTime.UtcNow.AddDays(10 - i),
                    EndDate = DateTime.UtcNow.AddDays(12 - i),
                    Location = $"Location {i + 1}",
                    Description = $"Description {i + 1}",
                    ResearchAxisId = researchAxis.Id,
                    CreatedBy = Guid.NewGuid(),
                    CreatedAtUtc = DateTime.UtcNow
                };
                DbContext.Events.Add(evt);
            }
            await DbContext.SaveChangesAsync();

            // 3. Retrieve first page with limit 2
            var firstPage = await EventsRepository.GetEventsAsync(null, null, 1, 2, null);
            Assert.Equal(2, firstPage.Count);

            // 4. Retrieve second page with limit 2
            var secondPage = await EventsRepository.GetEventsAsync(null, null, 2, 2, null);
            Assert.Equal(2, secondPage.Count);

            // 5. Verify no overlap
            var firstPageIds = firstPage.Select(e => e.Id).ToList();
            var secondPageIds = secondPage.Select(e => e.Id).ToList();
            Assert.Empty(firstPageIds.Intersect(secondPageIds));
        }
    }
}
