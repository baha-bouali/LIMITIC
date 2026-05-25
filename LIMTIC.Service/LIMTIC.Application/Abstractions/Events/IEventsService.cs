using LIMTIC.Application.Contracts.Commands.Events;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Events;
using LIMTIC.Application.DTOs.Storage;

namespace LIMTIC.Application.Abstractions.Events
{
    public interface IEventsService
    {
        Task<Result<(List<EventDto> Items, int Total)>> GetEventsAsync(string? status, string? type, int page, int limit, string? q);
        Task<Result<EventDto>> GetEventByIdAsync(Guid eventId);
        Task<Result<EventDto>> CreateEventAsync(CreateEventCommand command);
        Task<Result<EventDto>> UpdateEventAsync(UpdateEventCommand command);
        Task<Result<bool>> DeleteEventAsync(Guid eventId);
        Task<Result<SpeakerDto>> AddSpeakerAsync(CreateSpeakerCommand command);
        Task<Result<SpeakerDto>> UpdateSpeakerAsync(UpdateSpeakerCommand command);
        Task<Result<bool>> DeleteSpeakerAsync(Guid eventId, Guid speakerId);
        Task<Result<bool>> UploadEventPhotosAsync(Guid eventId, List<(Stream Stream, string FileName)> files);
        Task<Result<FileDownloadDto>> GetEventPhotoAsync(Guid eventId, int index);
    }
}
