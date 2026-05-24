using System.Net;
using LIMTIC.Domain.Entities.Events;
using LIMTIC.Domain.Entities.ResearchAxis;
using LIMTIC.Domain.Enums;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;
using LIMTIC.Infrastructure.Data;
using LIMTIC.WebAPI.Models.Events.AddSpeaker;
using LIMTIC.WebAPI.Models.Events.CreateEvent;
using LIMTIC.WebAPI.Models.Events.UpdateEvent;
using LIMTIC.WebAPI.Models.Events.UpdateSpeaker;
using Microsoft.Extensions.DependencyInjection;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class EventsE2ETests : BaseE2ETests
    {
        public EventsE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture) : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
        {
        }

        private void SeedEventsData()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var researchAxis = new ResearchAxisEntity
            {
                Id = Guid.NewGuid(),
                Title = "AI Research",
                Description = "Research in Artificial Intelligence",
                Themes = new[] { "Machine Learning", "Deep Learning" },
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };
            db.ResearchAxes.Add(researchAxis);
            db.SaveChanges();

            var upcomingEvent = new EventEntity
            {
                Id = Guid.NewGuid(),
                Title = "Future AI Conference 2025",
                Type = EventType.Conference,
                StartDate = DateTime.UtcNow.AddDays(30),
                EndDate = DateTime.UtcNow.AddDays(32),
                Location = "Paris Convention Center",
                Description = "An upcoming international conference on AI",
                ResearchAxisId = researchAxis.Id,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            var speaker1 = new SpeakerEntity
            {
                Id = Guid.NewGuid(),
                EventId = upcomingEvent.Id,
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@university.edu",
                Institution = "MIT",
                Role = "Professor",
                Biography = "Expert in Machine Learning",
                Photo = null,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            upcomingEvent.Speakers = new List<SpeakerEntity> { speaker1 };

            var ongoingEvent = new EventEntity
            {
                Id = Guid.NewGuid(),
                Title = "Machine Learning Seminar",
                Type = EventType.Seminar,
                StartDate = DateTime.UtcNow.AddHours(-1),
                EndDate = DateTime.UtcNow.AddHours(2),
                Location = "Room 101, Building A",
                Description = "Current seminar on practical ML applications",
                ResearchAxisId = researchAxis.Id,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            var pastEvent = new EventEntity
            {
                Id = Guid.NewGuid(),
                Title = "Data Science Workshop 2024",
                Type = EventType.Workshop,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(-9),
                Location = "Virtual",
                Description = "Past workshop on data science fundamentals",
                ResearchAxisId = researchAxis.Id,
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            db.Events.AddRange(upcomingEvent, ongoingEvent, pastEvent);
            db.SaveChanges();
        }

        private Guid SeedResearchAxis()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var researchAxis = new ResearchAxisEntity
            {
                Id = Guid.NewGuid(),
                Title = "Advanced AI",
                Description = "Advanced AI research axis",
                Themes = new[] { "NLP", "Vision" },
                CreatedBy = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            db.ResearchAxes.Add(researchAxis);
            db.SaveChanges();

            return researchAxis.Id;
        }

        [Fact]
        public async Task GetEventsE2ETest()
        {
            // Steps:
            // 1. Seed events data
            // 2. Send GET request to retrieve all events
            // 3. Assert response contains events with pagination info
            // 4. Assert response structure and data

            // 1. Seed events data
            SeedEventsData();

            // 2. Send GET request to retrieve all events
            var response = await Client.GetEvents();

            // 3. Assert response is not null and successful
            Assert.NotNull(response);
            Assert.True(response.Success);

            // 4. Assert response structure and data
            Assert.NotNull(response.Items);
            Assert.NotEmpty(response.Items);
            Assert.True(response.Total > 0);
            Assert.Equal(1, response.Page);
            Assert.Equal(20, response.Limit);
        }

        [Fact]
        public async Task GetEventsFilterByStatusE2ETest()
        {
            // Steps:
            // 1. Seed events data
            // 2. Send GET request with status filter "Upcoming"
            // 3. Assert response contains only upcoming events
            // 4. Send GET request with status filter "Past"
            // 5. Assert response contains only past events

            // 1. Seed events data
            SeedEventsData();

            // 2. Send GET request with status filter "Upcoming"
            var upcomingResponse = await Client.GetEvents(status: "Upcoming");

            // 3. Assert response contains only upcoming events
            Assert.NotNull(upcomingResponse);
            Assert.True(upcomingResponse.Success);
            Assert.NotEmpty(upcomingResponse.Items);
            Assert.All(upcomingResponse.Items, evt => Assert.Equal("Upcoming", evt.Status));

            // 4. Send GET request with status filter "Past"
            var pastResponse = await Client.GetEvents(status: "Past");

            // 5. Assert response contains only past events
            Assert.NotNull(pastResponse);
            Assert.True(pastResponse.Success);
            Assert.NotEmpty(pastResponse.Items);
            Assert.All(pastResponse.Items, evt => Assert.Equal("Past", evt.Status));
        }

        [Fact]
        public async Task GetEventsFilterByTypeE2ETest()
        {
            // Steps:
            // 1. Seed events data
            // 2. Send GET request with type filter "Conference"
            // 3. Assert response contains only conference events
            // 4. Send GET request with type filter "Seminar"
            // 5. Assert response contains only seminar events

            // 1. Seed events data
            SeedEventsData();

            // 2. Send GET request with type filter "Conference"
            var conferenceResponse = await Client.GetEvents(type: "Conference");

            // 3. Assert response contains only conference events
            Assert.NotNull(conferenceResponse);
            Assert.True(conferenceResponse.Success);
            Assert.NotEmpty(conferenceResponse.Items);
            Assert.All(conferenceResponse.Items, evt => Assert.Equal("Conference", evt.Type));

            // 4. Send GET request with type filter "Seminar"
            var seminarResponse = await Client.GetEvents(type: "Seminar");

            // 5. Assert response contains only seminar events
            Assert.NotNull(seminarResponse);
            Assert.True(seminarResponse.Success);
            Assert.NotEmpty(seminarResponse.Items);
            Assert.All(seminarResponse.Items, evt => Assert.Equal("Seminar", evt.Type));
        }

        [Fact]
        public async Task GetEventsSearchByQueryE2ETest()
        {
            // Steps:
            // 1. Seed events data
            // 2. Send GET request with search query for "AI"
            // 3. Assert response contains events matching the search query
            // 4. Send GET request with search query for "Workshop"
            // 5. Assert response contains only matching events

            // 1. Seed events data
            SeedEventsData();

            // 2. Send GET request with search query for "AI"
            var aiSearchResponse = await Client.GetEvents(q: "AI");

            // 3. Assert response contains events matching the search query
            Assert.NotNull(aiSearchResponse);
            Assert.True(aiSearchResponse.Success);
            Assert.NotEmpty(aiSearchResponse.Items);
            Assert.All(aiSearchResponse.Items, evt =>
                Assert.True(evt.Title.Contains("AI", StringComparison.OrdinalIgnoreCase) ||
                           evt.Description.Contains("AI", StringComparison.OrdinalIgnoreCase)));

            // 4. Send GET request with search query for "Workshop"
            var workshopSearchResponse = await Client.GetEvents(q: "Workshop");

            // 5. Assert response contains only matching events
            Assert.NotNull(workshopSearchResponse);
            Assert.True(workshopSearchResponse.Success);
            Assert.NotEmpty(workshopSearchResponse.Items);
            Assert.All(workshopSearchResponse.Items, evt =>
                Assert.True(evt.Title.Contains("Workshop", StringComparison.OrdinalIgnoreCase) ||
                           evt.Description.Contains("Workshop", StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public async Task GetEventsPaginationE2ETest()
        {
            // Steps:
            // 1. Seed events data
            // 2. Send GET request with page 1 and limit 1
            // 3. Assert first page contains 1 event
            // 4. Send GET request with page 2 and limit 1
            // 5. Assert second page contains 1 event and is different from first page

            // 1. Seed events data
            SeedEventsData();

            // 2. Send GET request with page 1 and limit 1
            var firstPageResponse = await Client.GetEvents(page: 1, limit: 1);

            // 3. Assert first page contains 1 event
            Assert.NotNull(firstPageResponse);
            Assert.True(firstPageResponse.Success);
            Assert.Single(firstPageResponse.Items);
            var firstEventId = firstPageResponse.Items[0].Id;

            // 4. Send GET request with page 2 and limit 1
            var secondPageResponse = await Client.GetEvents(page: 2, limit: 1);

            // 5. Assert second page contains 1 event and is different from first page
            Assert.NotNull(secondPageResponse);
            Assert.True(secondPageResponse.Success);
            Assert.Single(secondPageResponse.Items);
            Assert.NotEqual(firstEventId, secondPageResponse.Items[0].Id);
        }

        [Fact]
        public async Task GetEventsIncludesSpeakersE2ETest()
        {
            // Steps:
            // 1. Seed events data (with speakers)
            // 2. Send GET request to retrieve events
            // 3. Assert response includes speaker data
            // 4. Assert speaker properties are correctly mapped

            // 1. Seed events data (with speakers)
            SeedEventsData();

            // 2. Send GET request to retrieve events
            var response = await Client.GetEvents();

            // 3. Assert response includes speaker data
            Assert.NotNull(response);
            Assert.True(response.Success);
            var eventWithSpeakers = response.Items.FirstOrDefault(e => e.Speakers.Any());
            Assert.NotNull(eventWithSpeakers);

            // 4. Assert speaker properties are correctly mapped
            var speaker = eventWithSpeakers.Speakers[0];
            Assert.NotNull(speaker.Id);
            Assert.NotNull(speaker.FirstName);
            Assert.NotNull(speaker.LastName);
            Assert.NotNull(speaker.Email);
            Assert.NotNull(speaker.Institution);
            Assert.NotNull(speaker.Role);
        }

        [Fact]
        public async Task GetEventsIsPublicEndpointE2ETest()
        {
            // Steps:
            // 1. Seed events data
            // 2. Send GET request without authentication
            // 3. Assert response is successful (public endpoint)

            // 1. Seed events data
            SeedEventsData();

            // 2. Send GET request without authentication
            var response = await Client.GetEventsFullHttpResponse();

            // 3. Assert response is successful (public endpoint)
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetEventsEmptyResponseE2ETest()
        {
            // Steps:
            // 1. Do not seed any events
            // 2. Send GET request to retrieve events
            // 3. Assert response contains empty items list
            // 4. Assert total count is 0

            // 1. No events seeded - using empty database (only admin user)

            // 2. Send GET request to retrieve events
            var response = await Client.GetEvents();

            // 3. Assert response contains empty items list
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Empty(response.Items);

            // 4. Assert total count is 0
            Assert.Equal(0, response.Total);
        }

        [Fact]
        public async Task GetEventsCombinedFiltersE2ETest()
        {
            // Steps:
            // 1. Seed events data
            // 2. Send GET request with multiple filters (status + type)
            // 3. Assert response contains events matching all filters
            // 4. Send GET request with filters + search query
            // 5. Assert response correctly applies all filters and search

            // 1. Seed events data
            SeedEventsData();

            // 2. Send GET request with multiple filters (status + type)
            var filteredResponse = await Client.GetEvents(status: "Upcoming", type: "Conference");

            // 3. Assert response contains events matching all filters
            Assert.NotNull(filteredResponse);
            Assert.True(filteredResponse.Success);
            Assert.NotEmpty(filteredResponse.Items);
            Assert.All(filteredResponse.Items, evt =>
            {
                Assert.Equal("Upcoming", evt.Status);
                Assert.Equal("Conference", evt.Type);
            });

            // 4. Send GET request with filters + search query
            var combinedResponse = await Client.GetEvents(status: "Upcoming", q: "AI");

            // 5. Assert response correctly applies all filters and search
            Assert.NotNull(combinedResponse);
            Assert.True(combinedResponse.Success);
            Assert.NotEmpty(combinedResponse.Items);
            Assert.All(combinedResponse.Items, evt =>
            {
                Assert.Equal("Upcoming", evt.Status);
                Assert.True(evt.Title.Contains("AI", StringComparison.OrdinalIgnoreCase) ||
                           evt.Description.Contains("AI", StringComparison.OrdinalIgnoreCase));
            });
        }

        [Fact]
        public async Task CreateUpdateDeleteEventE2ETest()
        {
            var accessToken = await LoginAsSuperAdmin();
            Assert.NotNull(accessToken);

            var researchAxisId = SeedResearchAxis();

            var createRequest = new CreateEventRequest
            {
                Type = "Conference",
                Title = "AI and Health",
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(8),
                Location = "IST Amphitheater",
                Description = "Conference on AI in healthcare",
                Program = "Opening and talks",
                ResearchAxisId = researchAxisId,
                Speakers = new List<CreateEventSpeakerRequest>
                {
                    new()
                    {
                        FirstName = "Nora",
                        LastName = "Hamdi",
                        Email = "nora.hamdi@test.com",
                        Institution = "INSAT",
                        Role = "Speaker",
                        Biography = "AI researcher"
                    }
                }
            };

            var createResponse = await Client.CreateEvent(createRequest, accessToken);
            Assert.NotNull(createResponse);
            Assert.True(createResponse.Success);
            Assert.NotNull(createResponse.Event);
            Assert.Equal("AI and Health", createResponse.Event.Title);

            var eventId = Guid.Parse(createResponse.Event.Id);

            var updateRequest = new UpdateEventRequest
            {
                Type = "Seminar",
                Title = "AI and Health - Updated",
                StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddDays(11),
                Location = "Main Hall",
                Description = "Updated description",
                Program = "Updated program",
                ResearchAxisId = researchAxisId
            };

            var updateResponse = await Client.UpdateEvent(eventId, updateRequest, accessToken);
            Assert.NotNull(updateResponse);
            Assert.True(updateResponse.Success);
            Assert.NotNull(updateResponse.Event);
            Assert.Equal("AI and Health - Updated", updateResponse.Event.Title);
            Assert.Equal("Seminar", updateResponse.Event.Type);

            var deleteResponse = await Client.DeleteEvent(eventId, accessToken);
            Assert.NotNull(deleteResponse);
            Assert.True(deleteResponse.Success);

            var eventsAfterDelete = await Client.GetEvents(q: "AI and Health - Updated");
            Assert.NotNull(eventsAfterDelete);
            Assert.True(eventsAfterDelete.Success);
            Assert.DoesNotContain(eventsAfterDelete.Items, e => e.Id == eventId.ToString());
        }

        [Fact]
        public async Task AddUpdateDeleteSpeakerE2ETest()
        {
            var accessToken = await LoginAsSuperAdmin();
            Assert.NotNull(accessToken);

            var researchAxisId = SeedResearchAxis();
            var createEventResponse = await Client.CreateEvent(new CreateEventRequest
            {
                Type = "Workshop",
                Title = "Speaker Operations Event",
                StartDate = DateTime.UtcNow.AddDays(4),
                EndDate = DateTime.UtcNow.AddDays(5),
                Location = "Lab 1",
                Description = "Event for speaker CRUD",
                ResearchAxisId = researchAxisId
            }, accessToken);

            Assert.NotNull(createEventResponse);
            Assert.True(createEventResponse.Success);
            Assert.NotNull(createEventResponse.Event);

            var eventId = Guid.Parse(createEventResponse.Event.Id);

            var addSpeakerResponse = await Client.AddSpeaker(eventId, new AddSpeakerRequest
            {
                FirstName = "Amine",
                LastName = "Ben Ali",
                Email = "amine.benali@test.com",
                Institution = "ENIT",
                Role = "Lecturer",
                Biography = "Initial biography"
            }, accessToken);

            Assert.NotNull(addSpeakerResponse);
            Assert.True(addSpeakerResponse.Success);
            Assert.NotNull(addSpeakerResponse.Speaker);
            Assert.Equal("Amine", addSpeakerResponse.Speaker.FirstName);

            var speakerId = Guid.Parse(addSpeakerResponse.Speaker.Id);

            var updateSpeakerResponse = await Client.UpdateSpeaker(eventId, speakerId, new UpdateSpeakerRequest
            {
                FirstName = "Amine",
                LastName = "Ben Ali",
                Email = "amine.benali@test.com",
                Institution = "ENIT",
                Role = "Keynote",
                Biography = "Updated biography"
            }, accessToken);

            Assert.NotNull(updateSpeakerResponse);
            Assert.True(updateSpeakerResponse.Success);
            Assert.NotNull(updateSpeakerResponse.Speaker);
            Assert.Equal("Keynote", updateSpeakerResponse.Speaker.Role);

            var deleteSpeakerResponse = await Client.DeleteSpeaker(eventId, speakerId, accessToken);
            Assert.NotNull(deleteSpeakerResponse);
            Assert.True(deleteSpeakerResponse.Success);

            var eventsResponse = await Client.GetEvents(q: "Speaker Operations Event");
            Assert.NotNull(eventsResponse);
            Assert.True(eventsResponse.Success);
            var targetEvent = eventsResponse.Items.Single(e => e.Id == eventId.ToString());
            Assert.DoesNotContain(targetEvent.Speakers, s => s.Id == speakerId.ToString());
        }
    }
}
