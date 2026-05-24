using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Events;

namespace LIMTIC.Application.Abstractions.Events
{
    public interface IEventsService
    {
        Task<Result<(List<EventDto> Items, int Total)>> GetEventsAsync(string? status, string? type, int page, int limit, string? q);
    }
}
