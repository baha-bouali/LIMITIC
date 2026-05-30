using LIMTIC.Domain.Entities.Events;

namespace LIMTIC.Domain.Abstractions.Events
{
    public interface IEventsRepository
    {
        Task<List<EventEntity>> GetEventsAsync(string? status, string? type, int page, int limit, string? q);
        Task<int> GetTotalEventsCountAsync(string? status, string? type, string? q);
        Task<EventEntity?> GetEventByIdAsync(Guid eventId);
        Task<bool> AddEventAsync(EventEntity eventEntity);
        Task<bool> UpdateEventAsync(EventEntity eventEntity);
        Task<bool> DeleteEventAsync(EventEntity eventEntity);
        Task<SpeakerEntity?> GetSpeakerByIdAsync(Guid eventId, Guid speakerId);
        Task<bool> AddSpeakerAsync(SpeakerEntity speakerEntity);
        Task<bool> UpdateSpeakerAsync(SpeakerEntity speakerEntity);
        Task<bool> DeleteSpeakerAsync(SpeakerEntity speakerEntity);
    }
}
