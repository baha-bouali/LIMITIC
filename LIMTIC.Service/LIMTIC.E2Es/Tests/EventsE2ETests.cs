using LIMTIC.Application.Contracts.Commands.Events;
using LIMTIC.Application.Contracts.Queries.Events;
using LIMTIC.Domain.Entities.ResearchAxis;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;
using LIMTIC.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class EventsE2ETests : BaseE2ETests
    {
        public EventsE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture) : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
        {
        }

        private async Task SeedEventsDataUsingEndpoints()
        {
            var accessToken = await LoginAsSuperAdmin();
            Assert.NotNull(accessToken);

            var researchAxisId = SeedResearchAxis();

            var upcoming = await Client.CreateEvent(new CreateEventCommand
            {
                Type = "Conference",
                Title = "Future AI Conference 2025",
                StartDate = DateTime.UtcNow.AddDays(30),
                EndDate = DateTime.UtcNow.AddDays(32),
                Location = "Paris Convention Center",
                Description = "An upcoming international conference on AI",
                ResearchAxisId = researchAxisId,
                Speakers = new List<CreateSpeakerItemCommand>
                {
                    new CreateSpeakerItemCommand
                    {
                        FirstName = "John",
                        LastName = "Smith",
                        Email = "john.smith@university.edu",
                        Institution = "MIT",
                        Role = "Professor",
                        Subject = "Machine Learning"
                    }
                }
            }, accessToken);

            var ongoing = await Client.CreateEvent(new CreateEventCommand
            {
                Type = "Seminar",
                Title = "Machine Learning Seminar",
                StartDate = DateTime.UtcNow.AddHours(-1),
                EndDate = DateTime.UtcNow.AddHours(2),
                Location = "Room 101, Building A",
                Description = "Current seminar on practical ML applications",
                ResearchAxisId = researchAxisId
            }, accessToken);

            var past = await Client.CreateEvent(new CreateEventCommand
            {
                Type = "Workshop",
                Title = "Data Science Workshop 2024",
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(-9),
                Location = "Virtual",
                Description = "Past workshop on data science fundamentals",
                ResearchAxisId = researchAxisId
            }, accessToken);

            Assert.NotNull(upcoming);
            Assert.NotNull(ongoing);
            Assert.NotNull(past);
            Assert.True(upcoming.Success && ongoing.Success && past.Success);
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
            await SeedEventsDataUsingEndpoints();

            // 2. Send GET request to retrieve all events
            var response = await Client.GetEvents(new GetEventsQuery());

            // 3. Assert response is not null and successful
            Assert.NotNull(response);
            Assert.True(response.Success);

            // 4. Assert response structure and data
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
            Assert.True(response.Pagination?.Total > 0);
            Assert.Equal(1, response.Pagination?.Page);
            Assert.Equal(20, response.Pagination?.Limit);
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
            await SeedEventsDataUsingEndpoints();

            // 2. Send GET request with status filter "Upcoming"
            var query = new GetEventsQuery
            {
                Status = "Upcoming"
            };
            var upcomingResponse = await Client.GetEvents(query);

            // 3. Assert response contains only upcoming events
            Assert.NotNull(upcomingResponse);
            Assert.True(upcomingResponse.Success);
            Assert.NotEmpty(upcomingResponse.Data);
            Assert.All(upcomingResponse.Data, evt => Assert.Equal("Upcoming", evt.Status));

            // 4. Send GET request with status filter "Past"
            query.Status = "Past";
            var pastResponse = await Client.GetEvents(query);

            // 5. Assert response contains only past events
            Assert.NotNull(pastResponse);
            Assert.True(pastResponse.Success);
            Assert.NotEmpty(pastResponse.Data);
            Assert.All(pastResponse.Data, evt => Assert.Equal("Past", evt.Status));
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
            await SeedEventsDataUsingEndpoints();

            // 2. Send GET request with type filter "Conference"
            var query = new GetEventsQuery
            {
                Type = "Conference"
            };
            var conferenceResponse = await Client.GetEvents(query);

            // 3. Assert response contains only conference events
            Assert.NotNull(conferenceResponse);
            Assert.True(conferenceResponse.Success);
            Assert.NotEmpty(conferenceResponse.Data);
            Assert.All(conferenceResponse.Data, evt => Assert.Equal("Conference", evt.Type));

            // 4. Send GET request with type filter "Seminar"
            query.Type = "Seminar";
            var seminarResponse = await Client.GetEvents(query);

            // 5. Assert response contains only seminar events
            Assert.NotNull(seminarResponse);
            Assert.True(seminarResponse.Success);
            Assert.NotEmpty(seminarResponse.Data);
            Assert.All(seminarResponse.Data, evt => Assert.Equal("Seminar", evt.Type));
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
            await SeedEventsDataUsingEndpoints();

            // 2. Send GET request with search query for "AI"
            var query = new GetEventsQuery
            {
                Q = "AI"
            };
            var aiSearchResponse = await Client.GetEvents(query);

            // 3. Assert response contains events matching the search query
            Assert.NotNull(aiSearchResponse);
            Assert.True(aiSearchResponse.Success);
            Assert.NotEmpty(aiSearchResponse.Data);
            Assert.All(aiSearchResponse.Data, evt =>
                Assert.True(evt.Title.Contains("AI", StringComparison.OrdinalIgnoreCase) ||
                           evt.Description.Contains("AI", StringComparison.OrdinalIgnoreCase)));

            // 4. Send GET request with search query for "Workshop"
            query = new GetEventsQuery
            {
                Q = "Workshop"
            };
            var workshopSearchResponse = await Client.GetEvents(query);

            // 5. Assert response contains only matching events
            Assert.NotNull(workshopSearchResponse);
            Assert.True(workshopSearchResponse.Success);
            Assert.NotEmpty(workshopSearchResponse.Data);
            Assert.All(workshopSearchResponse.Data, evt =>
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
            await SeedEventsDataUsingEndpoints();

            // 2. Send GET request with page 1 and limit 1
            var query = new GetEventsQuery
            {
                Page = 1,
                Limit = 1
            };
            var firstPageResponse = await Client.GetEvents(query);

            // 3. Assert first page contains 1 event
            Assert.NotNull(firstPageResponse);
            Assert.True(firstPageResponse.Success);
            Assert.Single(firstPageResponse.Data);
            var firstEventId = firstPageResponse.Data[0].Id;

            // 4. Send GET request with page 2 and limit 1
            query.Page = 2;
            var secondPageResponse = await Client.GetEvents(query);

            // 5. Assert second page contains 1 event and is different from first page
            Assert.NotNull(secondPageResponse);
            Assert.True(secondPageResponse.Success);
            Assert.Single(secondPageResponse.Data);
            Assert.NotEqual(firstEventId, secondPageResponse.Data[0].Id);
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
            await SeedEventsDataUsingEndpoints();

            // 2. Send GET request to retrieve events
            var response = await Client.GetEvents(new GetEventsQuery());

            // 3. Assert response includes speaker data
            Assert.NotNull(response);
            Assert.True(response.Success);
            var eventWithSpeakers = response.Data?.FirstOrDefault(e => e.Speakers.Any());
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
        public async Task GetEventsEmptyResponseE2ETest()
        {
            // Steps:
            // 1. Do not seed any events
            // 2. Send GET request to retrieve events
            // 3. Assert response contains empty items list
            // 4. Assert total count is 0

            // 1. No events seeded - using empty database (only admin user)

            // 2. Send GET request to retrieve events
            var response = await Client.GetEvents(new GetEventsQuery());

            // 3. Assert response contains empty items list
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Empty(response.Data);

            // 4. Assert total count is 0
            Assert.Equal(0, response.Pagination?.Total);
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
            await SeedEventsDataUsingEndpoints();

            // 2. Send GET request with multiple filters (status + type)
            var query = new GetEventsQuery
            {
                Status = "Upcoming",
                Type = "Conference"
            };
            var filteredResponse = await Client.GetEvents(query);

            // 3. Assert response contains events matching all filters
            Assert.NotNull(filteredResponse);
            Assert.True(filteredResponse.Success);
            Assert.NotEmpty(filteredResponse.Data);
            Assert.All(filteredResponse.Data, evt =>
            {
                Assert.Equal("Upcoming", evt.Status);
                Assert.Equal("Conference", evt.Type);
            });

            // 4. Send GET request with filters + search query
            query = new GetEventsQuery
            {
                Status = "Upcoming",
                Q = "AI"
            };
            var combinedResponse = await Client.GetEvents(query);

            // 5. Assert response correctly applies all filters and search
            Assert.NotNull(combinedResponse);
            Assert.True(combinedResponse.Success);
            Assert.NotEmpty(combinedResponse.Data);
            Assert.All(combinedResponse.Data, evt =>
            {
                Assert.Equal("Upcoming", evt.Status);
                Assert.True(evt.Title.Contains("AI", StringComparison.OrdinalIgnoreCase) ||
                           evt.Description.Contains("AI", StringComparison.OrdinalIgnoreCase));
            });
        }

        [Fact]
        public async Task CreateUpdateDeleteEventE2ETest()
        {
            // Steps:
            // 1. Authenticate as SuperAdmin
            // 2. Seed a research axis for event creation
            // 3. Create an event with one speaker
            // 4. Update the created event fields
            // 5. Delete the event
            // 6. Verify deleted event is not returned by GET /api/events

            var accessToken = await LoginAsSuperAdmin();
            Assert.NotNull(accessToken);

            var researchAxisId = SeedResearchAxis();

            var createRequest = new CreateEventCommand
            {
                Type = "Conference",
                Title = "AI and Health",
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(8),
                Location = "IST Amphitheater",
                Description = "Conference on AI in healthcare",
                Program = "Opening and talks",
                ResearchAxisId = researchAxisId,
                Speakers = new List<CreateSpeakerItemCommand>
                {
                    new CreateSpeakerItemCommand()
                    {
                        FirstName = "Nora",
                        LastName = "Hamdi",
                        Email = "nora.hamdi@test.com",
                        Institution = "INSAT",
                        Role = "Speaker",
                        Subject = "Ai"
                    }
                }
            };

            var createResponse = await Client.CreateEvent(createRequest, accessToken);
            Assert.NotNull(createResponse);
            Assert.True(createResponse.Success);
            Assert.NotNull(createResponse.Data);
            Assert.Equal("AI and Health", createResponse.Data.Title);

            var eventId = Guid.Parse(createResponse.Data.Id);

            var updateRequest = new UpdateEventCommand
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
            Assert.NotNull(updateResponse.Data);
            Assert.Equal("AI and Health - Updated", updateResponse.Data.Title);
            Assert.Equal("Seminar", updateResponse.Data.Type);

            var deleteResponse = await Client.DeleteEvent(eventId, accessToken);
            Assert.NotNull(deleteResponse);
            Assert.True(deleteResponse.Success);

            var query = new GetEventsQuery
            {
                Q = "AI and Health - Updated"
            };
            var eventsAfterDelete = await Client.GetEvents(query);
            Assert.NotNull(eventsAfterDelete);
            Assert.True(eventsAfterDelete.Success);
            Assert.DoesNotContain(eventsAfterDelete.Data, e => e.Id == eventId.ToString());
        }

        [Fact]
        public async Task AddUpdateDeleteSpeakerE2ETest()
        {
            // Steps:
            // 1. Authenticate as SuperAdmin
            // 2. Seed a research axis and create a parent event
            // 3. Add a speaker to the event
            // 4. Update the created speaker
            // 5. Delete the speaker
            // 6. Verify deleted speaker is not returned in event speakers list

            var accessToken = await LoginAsSuperAdmin();
            Assert.NotNull(accessToken);

            var researchAxisId = SeedResearchAxis();
            var createEventResponse = await Client.CreateEvent(new CreateEventCommand
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
            Assert.NotNull(createEventResponse.Data);

            var eventId = Guid.Parse(createEventResponse.Data.Id);

            var addSpeakerResponse = await Client.AddSpeaker(eventId, new CreateSpeakerCommand
            {
                FirstName = "Amine",
                LastName = "Ben Ali",
                Email = "amine.benali@test.com",
                Institution = "ENIT",
                Role = "Lecturer",
                Subject = "Initial biography"
            }, accessToken);

            Assert.NotNull(addSpeakerResponse);
            Assert.True(addSpeakerResponse.Success);
            Assert.NotNull(addSpeakerResponse.Data);
            Assert.Equal("Amine", addSpeakerResponse.Data.FirstName);

            var speakerId = Guid.Parse(addSpeakerResponse.Data.Id);

            var updateSpeakerResponse = await Client.UpdateSpeaker(eventId, speakerId, new UpdateSpeakerCommand
            {
                FirstName = "Amine",
                LastName = "Ben Ali",
                Email = "amine.benali@test.com",
                Institution = "ENIT",
                Role = "Keynote",
               Subject = "update subject"
            }, accessToken);

            Assert.NotNull(updateSpeakerResponse);
            Assert.True(updateSpeakerResponse.Success);
            Assert.NotNull(updateSpeakerResponse.Data);
            Assert.Equal("Keynote", updateSpeakerResponse.Data.Role);

            var deleteSpeakerResponse = await Client.DeleteSpeaker(eventId, speakerId, accessToken);
            Assert.NotNull(deleteSpeakerResponse);
            Assert.True(deleteSpeakerResponse.Success);

            var query = new GetEventsQuery
            {
                Q = "Speaker Operations Event"
            };
            var eventsResponse = await Client.GetEvents(query);
            Assert.NotNull(eventsResponse);
            Assert.True(eventsResponse.Success);
            var targetEvent = eventsResponse.Data?.Single(e => e.Id == eventId.ToString());
            Assert.DoesNotContain(targetEvent.Speakers, s => s.Id == speakerId.ToString());
        }
    }
}
