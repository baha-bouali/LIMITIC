using FluentValidation;
using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Events;
using LIMTIC.Application.Abstractions.Storage;
using LIMTIC.Application.Contracts.Commands.Events;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Events;
using LIMTIC.Application.DTOs.Storage;
using LIMTIC.Application.Helpers;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Events;
using LIMTIC.Domain.Enums;
using System.IO;

namespace LIMTIC.Application.Services.Events
{
    public class EventsService : IEventsService
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly IValidator<CreateEventCommand> _createEventCommandValidator;
        private readonly IValidator<UpdateEventCommand> _updateEventCommandValidator;
        private readonly IValidator<CreateSpeakerCommand> _createSpeakerCommandValidator;
        private readonly IValidator<UpdateSpeakerCommand> _updateSpeakerCommandValidator;
        private readonly IAuditLogsRepository _auditLogsRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBlobStorageService _blobStorageService;

        public EventsService(
            IEventsRepository eventsRepository,
            IValidator<CreateEventCommand> createEventCommandValidator,
            IValidator<UpdateEventCommand> updateEventCommandValidator,
            IValidator<CreateSpeakerCommand> createSpeakerCommandValidator,
            IValidator<UpdateSpeakerCommand> updateSpeakerCommandValidator,
            IAuditLogsRepository auditLogsRepository,
            ICurrentUserService currentUserService,
            IBlobStorageService blobStorageService)
        {
            _eventsRepository = eventsRepository;
            _createEventCommandValidator = createEventCommandValidator;
            _updateEventCommandValidator = updateEventCommandValidator;
            _createSpeakerCommandValidator = createSpeakerCommandValidator;
            _updateSpeakerCommandValidator = updateSpeakerCommandValidator;
            _auditLogsRepository = auditLogsRepository;
            _currentUserService = currentUserService;
            _blobStorageService = blobStorageService;
        }

        public async Task<Result<(List<EventDto> Items, int Total)>> GetEventsAsync(string? status, string? type, int page, int limit, string? q)
        {
            try
            {
                var events = await _eventsRepository.GetEventsAsync(status, type, page, limit, q);

                var eventDtos = events.Select(MapEvent).ToList();

                var total = await _eventsRepository.GetTotalEventsCountAsync(status, type, q);

                return Result<(List<EventDto>, int)>.SuccessResult((eventDtos, total));
            }
            catch (Exception ex)
            {
                return Result<(List<EventDto>, int)>.FailureResult($"Error retrieving events: {ex.Message}");
            }
        }

        public async Task<Result<EventDto>> GetEventByIdAsync(Guid eventId)
        {
            try
            {
                var eventEntity = await _eventsRepository.GetEventByIdAsync(eventId);
                if (eventEntity == null)
                {
                    return Result<EventDto>.FailureResult("Event not found");
                }

                return Result<EventDto>.SuccessResult(MapEvent(eventEntity));
            }
            catch (Exception ex)
            {
                return Result<EventDto>.FailureResult($"Error retrieving event: {ex.Message}");
            }
        }

        public async Task<Result<EventDto>> CreateEventAsync(CreateEventCommand command)
        {
            var validationResult = _createEventCommandValidator.Validate(command);
            if (!validationResult.IsValid)
                return Result<EventDto>.ValidationFailureResult(ValidationHelper.ParseValidationErrors(validationResult));

            var eventEntity = new EventEntity
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                Type = command.Type,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                Location = command.Location,
                Description = command.Description,
                Program = command.Program,
                ResearchAxisId = command.ResearchAxisId,
                Speakers = command.Speakers?.Select(s => new SpeakerEntity
                {
                    Id = Guid.NewGuid(),
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    Institution = s.Institution,
                    Role = s.Role,
                    Subject = s.Subject
                }).ToList()
            };

            var created = await _eventsRepository.AddEventAsync(eventEntity);
            if (!created)
                return Result<EventDto>.FailureResult("Failed to create event");

            var eventLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.CREATE, ResourceType.Event);
            await _auditLogsRepository.AddLog(eventLog);

            return Result<EventDto>.SuccessResult(MapEvent(eventEntity));
        }

        public async Task<Result<EventDto>> UpdateEventAsync(UpdateEventCommand command)
        {
            var validationResult = _updateEventCommandValidator.Validate(command);
            if (!validationResult.IsValid)
                return Result<EventDto>.ValidationFailureResult(ValidationHelper.ParseValidationErrors(validationResult));

            var existingEvent = await _eventsRepository.GetEventByIdAsync(command.Id);
            if (existingEvent == null)
                return Result<EventDto>.FailureResult("Event not found");

            existingEvent.Title = command.Title;
            existingEvent.Type = command.Type;
            existingEvent.StartDate = command.StartDate;
            existingEvent.EndDate = command.EndDate;
            existingEvent.Location = command.Location;
            existingEvent.Description = command.Description;
            existingEvent.Program = command.Program;
            existingEvent.ResearchAxisId = command.ResearchAxisId;

            var updated = await _eventsRepository.UpdateEventAsync(existingEvent);
            if (!updated)
                return Result<EventDto>.FailureResult("Failed to update event");

            var eventLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.UPDATE, ResourceType.Event);
            await _auditLogsRepository.AddLog(eventLog);

            return Result<EventDto>.SuccessResult(MapEvent(existingEvent));
        }

        public async Task<Result<bool>> DeleteEventAsync(Guid eventId)
        {
            var existingEvent = await _eventsRepository.GetEventByIdAsync(eventId);
            if (existingEvent == null)
                return Result<bool>.FailureResult("Event not found");

            var deleted = await _eventsRepository.DeleteEventAsync(existingEvent);
            if (!deleted)
                return Result<bool>.FailureResult("Failed to delete event");

            var eventLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.DELETE, ResourceType.Event);
            await _auditLogsRepository.AddLog(eventLog);

            return Result<bool>.SuccessResult(true);
        }

        public async Task<Result<SpeakerDto>> AddSpeakerAsync(CreateSpeakerCommand command)
        {
            var validationResult = _createSpeakerCommandValidator.Validate(command);
            if (!validationResult.IsValid)
                return Result<SpeakerDto>.ValidationFailureResult(ValidationHelper.ParseValidationErrors(validationResult));

            var existingEvent = await _eventsRepository.GetEventByIdAsync(command.EventId);
            if (existingEvent == null)
                return Result<SpeakerDto>.FailureResult("Event not found");

            var speaker = new SpeakerEntity
            {
                Id = Guid.NewGuid(),
                EventId = command.EventId,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                Institution = command.Institution,
                Role = command.Role,
                Subject= command.Subject
            };

            var created = await _eventsRepository.AddSpeakerAsync(speaker);
            if (!created)
                return Result<SpeakerDto>.FailureResult("Failed to create speaker");

            var speakerLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.CREATE, ResourceType.Event);
            await _auditLogsRepository.AddLog(speakerLog);

            return Result<SpeakerDto>.SuccessResult(MapSpeaker(speaker));
        }

        public async Task<Result<SpeakerDto>> UpdateSpeakerAsync(UpdateSpeakerCommand command)
        {
            var validationResult = _updateSpeakerCommandValidator.Validate(command);
            if (!validationResult.IsValid)
                return Result<SpeakerDto>.ValidationFailureResult(ValidationHelper.ParseValidationErrors(validationResult));

            var existingSpeaker = await _eventsRepository.GetSpeakerByIdAsync(command.EventId, command.SpeakerId);
            if (existingSpeaker == null)
                return Result<SpeakerDto>.FailureResult("Speaker not found");

            existingSpeaker.FirstName = command.FirstName;
            existingSpeaker.LastName = command.LastName;
            existingSpeaker.Email = command.Email;
            existingSpeaker.Institution = command.Institution;
            existingSpeaker.Role = command.Role;
            existingSpeaker.Subject = command.Subject;

            var updated = await _eventsRepository.UpdateSpeakerAsync(existingSpeaker);
            if (!updated)
                return Result<SpeakerDto>.FailureResult("Failed to update speaker");

            var speakerLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.UPDATE, ResourceType.Event);
            await _auditLogsRepository.AddLog(speakerLog);

            return Result<SpeakerDto>.SuccessResult(MapSpeaker(existingSpeaker));
        }

        public async Task<Result<bool>> DeleteSpeakerAsync(Guid eventId, Guid speakerId)
        {
            var existingSpeaker = await _eventsRepository.GetSpeakerByIdAsync(eventId, speakerId);
            if (existingSpeaker == null)
                return Result<bool>.FailureResult("Speaker not found");

            var deleted = await _eventsRepository.DeleteSpeakerAsync(existingSpeaker);
            if (!deleted)
                return Result<bool>.FailureResult("Failed to delete speaker");

            var speakerLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.DELETE, ResourceType.Event);
            await _auditLogsRepository.AddLog(speakerLog);

            return Result<bool>.SuccessResult(true);
        }

        public async Task<Result<bool>> UploadEventPhotosAsync(Guid eventId, List<(Stream Stream, string FileName)> files)
        {
            if (files == null || files.Count == 0)
                return Result<bool>.FailureResult("No files provided");

            var eventEntity = await _eventsRepository.GetEventByIdAsync(eventId);
            if (eventEntity == null)
                return Result<bool>.FailureResult("Event not found");

            foreach (var file in files)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext is not (".jpg" or ".jpeg" or ".png" or ".webp" or ".gif"))
                    return Result<bool>.FailureResult($"Unsupported image format: {file.FileName}");

                var blobName = $"events/{eventId}/photos/{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var uploaded = await _blobStorageService.UploadStreamAsync(file.Stream, "media", blobName, overwrite: true);
                if (!uploaded)
                    return Result<bool>.FailureResult($"Failed to upload photo: {file.FileName}");

                eventEntity.PhotoFileNames.Add(blobName);
            }

            var updated = await _eventsRepository.UpdateEventAsync(eventEntity);
            return updated
                ? Result<bool>.SuccessResult(true)
                : Result<bool>.FailureResult("Failed to save event photo references");
        }

        public async Task<Result<FileDownloadDto>> GetEventPhotoAsync(Guid eventId, int index)
        {
            var eventEntity = await _eventsRepository.GetEventByIdAsync(eventId);
            if (eventEntity == null || eventEntity.PhotoFileNames == null || index < 0 || index >= eventEntity.PhotoFileNames.Count)
                return Result<FileDownloadDto>.FailureResult("Photo not found");

            var blobName = eventEntity.PhotoFileNames[index];
            try
            {
                var stream = await _blobStorageService.GetStreamAsync("media", blobName);
                var fileName = Path.GetFileName(blobName);
                return Result<FileDownloadDto>.SuccessResult(new FileDownloadDto
                {
                    Stream = stream,
                    FileName = fileName,
                    ContentType = ResolveContentType(fileName)
                });
            }
            catch (FileNotFoundException)
            {
                return Result<FileDownloadDto>.FailureResult("Photo file not found in blob storage");
            }
        }

        private static EventDto MapEvent(EventEntity eventEntity)
        {
            return new EventDto
            {
                Id = eventEntity.Id.ToString(),
                Type = eventEntity.Type.ToString(),
                Title = eventEntity.Title,
                StartDate = eventEntity.StartDate,
                EndDate = eventEntity.EndDate,
                Location = eventEntity.Location,
                Status = eventEntity.Status.ToString(),
                Description = eventEntity.Description,
                Program = eventEntity.Program,
                PhotoFileNames = eventEntity.PhotoFileNames ?? [],
                Speakers = eventEntity.Speakers?.Select(MapSpeaker).ToList()
            };
        }

        private static SpeakerDto MapSpeaker(SpeakerEntity speaker)
        {
            return new SpeakerDto
            {
                Id = speaker.Id.ToString(),
                FirstName = speaker.FirstName,
                LastName = speaker.LastName,
                Email = speaker.Email,
                Institution = speaker.Institution,
                Role = speaker.Role,
                Subject = speaker.Subject
            };
        }

        private static string ResolveContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }
    }
}
