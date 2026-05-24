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

        [Fact]
        public async Task EventCrudAsyncWorksCorrectly()
        {
            // Steps:
            // 1. Create and save a research axis
            // 2. Create an event through repository
            // 3. Read it back and verify initial values
            // 4. Update event fields and save changes
            // 5. Read updated event and verify new values
            // 6. Delete event and verify it no longer exists

            var researchAxis = new ResearchAxisEntity
            {
                Id = Guid.NewGuid(),
                Title = "Systems",
                Description = "Systems research",
                Themes = new[] { "Distributed Systems" },
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };
            DbContext.ResearchAxes.Add(researchAxis);
            await DbContext.SaveChangesAsync();

            var eventId = Guid.NewGuid();
            var eventEntity = new EventEntity
            {
                Id = eventId,
                Title = "Initial Event",
                Type = EventType.Seminar,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                Location = "Tunis",
                Description = "Initial description",
                ResearchAxisId = researchAxis.Id,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            var created = await EventsRepository.AddEventAsync(eventEntity);
            Assert.True(created);

            var storedEvent = await EventsRepository.GetEventByIdAsync(eventId);
            Assert.NotNull(storedEvent);
            Assert.Equal("Initial Event", storedEvent.Title);

            storedEvent.Title = "Updated Event";
            storedEvent.Description = "Updated description";
            var updated = await EventsRepository.UpdateEventAsync(storedEvent);
            Assert.True(updated);

            var updatedEvent = await EventsRepository.GetEventByIdAsync(eventId);
            Assert.NotNull(updatedEvent);
            Assert.Equal("Updated Event", updatedEvent.Title);
            Assert.Equal("Updated description", updatedEvent.Description);

            var deleted = await EventsRepository.DeleteEventAsync(updatedEvent);
            Assert.True(deleted);

            var deletedEvent = await EventsRepository.GetEventByIdAsync(eventId);
            Assert.Null(deletedEvent);
        }

        [Fact]
        public async Task SpeakerCrudAsyncWorksCorrectly()
        {
            // Steps:
            // 1. Create and save a research axis and parent event
            // 2. Create a speaker through repository
            // 3. Read it back and verify initial values
            // 4. Update speaker fields and save changes
            // 5. Read updated speaker and verify new values
            // 6. Delete speaker and verify it no longer exists

            var researchAxis = new ResearchAxisEntity
            {
                Id = Guid.NewGuid(),
                Title = "AI",
                Description = "AI research",
                Themes = new[] { "ML" },
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };
            DbContext.ResearchAxes.Add(researchAxis);

            var eventId = Guid.NewGuid();
            var eventEntity = new EventEntity
            {
                Id = eventId,
                Title = "Speaker Event",
                Type = EventType.Conference,
                StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddDays(12),
                Location = "Sousse",
                Description = "Speaker test event",
                ResearchAxisId = researchAxis.Id,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };
            DbContext.Events.Add(eventEntity);
            await DbContext.SaveChangesAsync();

            var speakerId = Guid.NewGuid();
            var speaker = new SpeakerEntity
            {
                Id = speakerId,
                EventId = eventId,
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@test.com",
                Institution = "INSAT",
                Role = "Speaker",
                Biography = "Initial bio",
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            var created = await EventsRepository.AddSpeakerAsync(speaker);
            Assert.True(created);

            var storedSpeaker = await EventsRepository.GetSpeakerByIdAsync(eventId, speakerId);
            Assert.NotNull(storedSpeaker);
            Assert.Equal("Jane", storedSpeaker.FirstName);

            storedSpeaker.Role = "Keynote Speaker";
            storedSpeaker.Biography = "Updated bio";
            var updated = await EventsRepository.UpdateSpeakerAsync(storedSpeaker);
            Assert.True(updated);

            var updatedSpeaker = await EventsRepository.GetSpeakerByIdAsync(eventId, speakerId);
            Assert.NotNull(updatedSpeaker);
            Assert.Equal("Keynote Speaker", updatedSpeaker.Role);
            Assert.Equal("Updated bio", updatedSpeaker.Biography);

            var deleted = await EventsRepository.DeleteSpeakerAsync(updatedSpeaker);
            Assert.True(deleted);

            var deletedSpeaker = await EventsRepository.GetSpeakerByIdAsync(eventId, speakerId);
            Assert.Null(deletedSpeaker);
        }
    }
}
