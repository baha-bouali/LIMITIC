using LIMTIC.Application.Abstractions.Events;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Events;
using LIMTIC.Domain.Abstractions;

namespace LIMTIC.Application.Services.Events
{
    public class EventsService : IEventsService
    {
        private readonly IEventsRepository _eventsRepository;

        public EventsService(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        public async Task<Result<(List<EventDto> Items, int Total)>> GetEventsAsync(string? status, string? type, int page, int limit, string? q)
        {
            try
            {
                var events = await _eventsRepository.GetEventsAsync(status, type, page, limit, q);

                var eventDtos = events.Select(e => new EventDto
                {
                    Id = e.Id.ToString(),
                    Type = e.Type.ToString(),
                    Title = e.Title,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Location = e.Location,
                    Status = e.Status.ToString(),
                    Description = e.Description,
                    Speakers = e.Speakers.Select(s => new SpeakerDto
                    {
                        Id = s.Id.ToString(),
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Email = s.Email,
                        Institution = s.Institution,
                        Role = s.Role,
                        Biography = s.Biography,
                    }).ToList()
                }).ToList();

                var total = await _eventsRepository.GetTotalEventsCountAsync(status, type, q);

                return Result<(List<EventDto>, int)>.SuccessResult((eventDtos, total));
            }
            catch (Exception ex)
            {
                return Result<(List<EventDto>, int)>.FailureResult($"Error retrieving events: {ex.Message}");
            }
        }
    }
}
