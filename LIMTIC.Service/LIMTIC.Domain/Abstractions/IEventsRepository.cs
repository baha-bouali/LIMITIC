using LIMTIC.Domain.Entities.Events;

namespace LIMTIC.Domain.Abstractions
{
    public interface IEventsRepository
    {
        Task<List<EventEntity>> GetEventsAsync(string? status, string? type, int page, int limit, string? q);
        Task<int> GetTotalEventsCountAsync(string? status, string? type, string? q);
    }
}
