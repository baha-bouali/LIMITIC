using LIMTIC.Application.Abstractions.Events;
using LIMTIC.Application.Contracts.Commands.Events;
using LIMTIC.Application.DTOs.Events;
using LIMTIC.Domain.Enums;
using LIMTIC.WebAPI.Models;
using LIMTIC.WebAPI.Models.Events.AddSpeaker;
using LIMTIC.WebAPI.Models.Events.CreateEvent;
using LIMTIC.WebAPI.Models.Events.GetEvents;
using LIMTIC.WebAPI.Models.Events.GetEventById;
using LIMTIC.WebAPI.Models.Events.UpdateEvent;
using LIMTIC.WebAPI.Models.Events.UpdateSpeaker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers
{
    [ApiController]
    [Route("api/events/")]
    public class EventsController : ControllerBase
    {
        private readonly IEventsService _eventsService;

        public EventsController(IEventsService eventsService)
        {
            _eventsService = eventsService;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents([FromQuery] string? status, [FromQuery] string? type, [FromQuery] int page = 1, [FromQuery] int limit = 20, [FromQuery] string? q = null)
        {
            if (page < 1)
                page = 1;
            if (limit < 1)
                limit = 20;

            var result = await _eventsService.GetEventsAsync(status, type, page, limit, q);

            if (result.Success)
            {
                var (items, total) = result.Data;
                return Ok(new GetEventsResponse
                {
                    Success = true,
                    Items = items,
                    Total = total,
                    Page = page,
                    Limit = limit
                });
            }
            else
            {
                return BadRequest(new GetEventsResponse
                {
                    Success = false,
                    Message = result.Message,
                    Items = new List<EventDto>(),
                    Total = 0,
                    Page = page,
                    Limit = limit
                });
            }
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventById(Guid id)
        {
            var result = await _eventsService.GetEventByIdAsync(id);

            if (result.Success)
            {
                return Ok(new GetEventResponse
                {
                    Success = true,
                    Event = result.Data
                });
            }

            return NotFound(new GetEventResponse
            {
                Success = false,
                Message = result.Message
            });
        }

        [HttpPost("")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            if (!Enum.TryParse<EventType>(request.Type, true, out var eventType))
            {
                return BadRequest(new CreateEventResponse
                {
                    Success = false,
                    Message = "Invalid event type"
                });
            }

            var command = new CreateEventCommand
            {
                Type = eventType,
                Title = request.Title,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Location = request.Location,
                Description = request.Description,
                Program = request.Program,
                ResearchAxisId = request.ResearchAxisId,
                Speakers = request.Speakers?.Select(s => new CreateSpeakerItemCommand
                {
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    Institution = s.Institution,
                    Role = s.Role,
                    Subject = s.Subject
                }).ToList()
            };

            var result = await _eventsService.CreateEventAsync(command);
            if (result.Success)
            {
                return Ok(new CreateEventResponse
                {
                    Success = true,
                    Event = result.Data
                });
            }

            return BadRequest(new CreateEventResponse
            {
                Success = false,
                Message = result.Message,
                ValidationErrors = result.ValidationErrors
            });
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventRequest request)
        {
            if (!Enum.TryParse<EventType>(request.Type, true, out var eventType))
            {
                return BadRequest(new UpdateEventResponse
                {
                    Success = false,
                    Message = "Invalid event type"
                });
            }

            var command = new UpdateEventCommand
            {
                Id = id,
                Type = eventType,
                Title = request.Title,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Location = request.Location,
                Description = request.Description,
                Program = request.Program,
                ResearchAxisId = request.ResearchAxisId
            };

            var result = await _eventsService.UpdateEventAsync(command);
            if (result.Success)
            {
                return Ok(new UpdateEventResponse
                {
                    Success = true,
                    Event = result.Data
                });
            }

            return BadRequest(new UpdateEventResponse
            {
                Success = false,
                Message = result.Message,
                ValidationErrors = result.ValidationErrors
            });
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var result = await _eventsService.DeleteEventAsync(id);
            if (result.Success)
            {
                return Ok(new BaseResponse
                {
                    Success = true
                });
            }

            return BadRequest(new BaseResponse
            {
                Success = false,
                Message = result.Message,
                ValidationErrors = result.ValidationErrors
            });
        }

        [HttpPost("{id:guid}/speakers")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> AddSpeaker(Guid id, [FromBody] AddSpeakerRequest request)
        {
            var command = new CreateSpeakerCommand
            {
                EventId = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Institution = request.Institution,
                Role = request.Role,
                Subject = request.Subject
            };

            var result = await _eventsService.AddSpeakerAsync(command);
            if (result.Success)
            {
                return Ok(new AddSpeakerResponse
                {
                    Success = true,
                    Speaker = result.Data
                });
            }

            return BadRequest(new AddSpeakerResponse
            {
                Success = false,
                Message = result.Message,
                ValidationErrors = result.ValidationErrors
            });
        }

        [HttpPut("{id:guid}/speakers/{speakerId:guid}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateSpeaker(Guid id, Guid speakerId, [FromBody] UpdateSpeakerRequest request)
        {
            var command = new UpdateSpeakerCommand
            {
                EventId = id,
                SpeakerId = speakerId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Institution = request.Institution,
                Role = request.Role,
                Subject = request.Subject
            };

            var result = await _eventsService.UpdateSpeakerAsync(command);
            if (result.Success)
            {
                return Ok(new UpdateSpeakerResponse
                {
                    Success = true,
                    Speaker = result.Data
                });
            }

            return BadRequest(new UpdateSpeakerResponse
            {
                Success = false,
                Message = result.Message,
                ValidationErrors = result.ValidationErrors
            });
        }

        [HttpPost("{id:guid}/photos")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UploadPhotos(Guid id, [FromForm] IFormFileCollection files)
        {
            if (files == null || files.Count == 0)
                return BadRequest(new { Success = false, Message = "No files uploaded" });

            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "events");
            Directory.CreateDirectory(uploadDir);

            var savedFileNames = new List<string>();
            foreach (var file in files)
            {
                if (file.Length == 0) continue;
                var ext = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadDir, fileName);
                await using var stream = System.IO.File.Create(filePath);
                await file.CopyToAsync(stream);
                savedFileNames.Add(fileName);
            }

            var result = await _eventsService.AddPhotosAsync(id, savedFileNames);
            if (result.Success)
                return Ok(new { Success = true, Event = result.Data });

            return BadRequest(new { Success = false, Message = result.Message });
        }

        [HttpDelete("{id:guid}/speakers/{speakerId:guid}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteSpeaker(Guid id, Guid speakerId)
        {
            var result = await _eventsService.DeleteSpeakerAsync(id, speakerId);
            if (result.Success)
            {
                return Ok(new BaseResponse
                {
                    Success = true
                });
            }

            return BadRequest(new BaseResponse
            {
                Success = false,
                Message = result.Message,
                ValidationErrors = result.ValidationErrors
            });
        }
    }
}
