using LIMTIC.Application.Abstractions.Events;
using LIMTIC.Application.Contracts.Commands.Events;
using LIMTIC.Application.Contracts.Queries.Events;
using LIMTIC.Application.DTOs.Events;
using LIMTIC.Application.DTOs.Storage;
using LIMTIC.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers.Events
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents(GetEventsQuery getEventsQuery)
        {
            if (getEventsQuery.Page < 1)
                getEventsQuery.Page = 1;
            if (getEventsQuery.Limit < 1)
                getEventsQuery.Limit = 20;

            var result = await _eventsService.GetEventsAsync(getEventsQuery);

            if (result.Success)
            {
                var (items, total) = result.Data;
                return Ok(new BaseResponse<List<EventDto>>
                {
                    Success = true,
                    Data = items,
                    Pagination = new PaginationResponse
                    {
                        Page = getEventsQuery.Page,
                        Limit = getEventsQuery.Limit,
                        Total = total
                    }
                });
            }
            else
            {
                return BadRequest(new BaseResponse<List<EventDto>>
                {
                    Success = false,
                    Message = result.Message,
                    Data = new List<EventDto>(),
                    Pagination = new PaginationResponse
                    {
                        Page = getEventsQuery.Page,
                        Limit = getEventsQuery.Limit,
                        Total = 0
                    }
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
                return Ok(new BaseResponse<EventDto>
                {
                    Success = true,
                    Data = result.Data
                });
            }

            return NotFound(new BaseResponse<EventDto>
            {
                Success = false,
                Message = result.Message
            });
        }

        [HttpPost("")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventCommand command)
        {
            var result = await _eventsService.CreateEventAsync(command);
            if (result.Success)
            {
                return Ok(new BaseResponse<EventDto>
                {
                    Success = true,
                    Data = result.Data
                });
            }

            return BadRequest(new BaseResponse<EventDto>
            {
                Success = false,
                Message = result.Message,
                ValidationErrors = result.ValidationErrors
            });
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventCommand command)
        {
            var result = await _eventsService.UpdateEventAsync(command);
            if (result.Success)
            {
                return Ok(new BaseResponse<EventDto>
                {
                    Success = true,
                    Data = result.Data
                });
            }

            return BadRequest(new BaseResponse<EventDto>
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
                return Ok(new BaseResponse<bool>
                {
                    Success = true
                });
            }

            return BadRequest(new BaseResponse<bool>
            {
                Success = false,
                Message = result.Message,
                ValidationErrors = result.ValidationErrors
            });
        }

        [HttpPost("{id:guid}/speakers")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> AddSpeaker([FromBody] CreateSpeakerCommand command)
        {
            var result = await _eventsService.AddSpeakerAsync(command);
            if (result.Success)
            {
                return Ok(new BaseResponse<SpeakerDto>
                {
                    Success = true,
                    Data = result.Data
                });
            }

            return BadRequest(new BaseResponse<SpeakerDto>
            {
                Success = false,
                Message = result.Message,
                ValidationErrors = result.ValidationErrors
            });
        }

        [HttpPut("{id:guid}/speakers/{speakerId:guid}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateSpeaker([FromBody] UpdateSpeakerCommand command)
        {
            var result = await _eventsService.UpdateSpeakerAsync(command);
            if (result.Success)
            {
                return Ok(new BaseResponse<SpeakerDto>
                {
                    Success = true,
                    Data = result.Data
                });
            }

            return BadRequest(new BaseResponse<SpeakerDto>
            {
                Success = false,
                Message = result.Message,
                ValidationErrors = result.ValidationErrors
            });
        }

        [HttpDelete("{id:guid}/speakers/{speakerId:guid}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteSpeaker(Guid id, Guid speakerId)
        {
            var result = await _eventsService.DeleteSpeakerAsync(id, speakerId);
            if (result.Success)
            {
                return Ok(new BaseResponse<bool>
                {
                    Success = true
                });
            }

            return BadRequest(new BaseResponse<bool>
            {
                Success = false,
                Message = result.Message,
                ValidationErrors = result.ValidationErrors
            });
        }

        [HttpPost("{id:guid}/photos")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UploadEventPhotos(Guid eventId, List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest(new BaseResponse<bool> { Success = false, Message = "No files provided" });

            var filePayloads = new List<(Stream Stream, string FileName)>();
            foreach (var file in files.Where(f => f != null && f.Length > 0))
                filePayloads.Add((file.OpenReadStream(), file.FileName));

            try
            {
                var result = await _eventsService.UploadEventPhotosAsync(eventId, filePayloads);
                if (!result.Success)
                    return BadRequest(new BaseResponse<bool> { Success = false, Message = result.Message });

                return Ok(new BaseResponse<bool> { Success = true });
            }
            finally
            {
                foreach (var payload in filePayloads)
                    payload.Stream.Dispose();
            }
        }

        [HttpGet("{id:guid}/photos/{index:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventPhoto(Guid eventId)
        {
            var result = await _eventsService.GetEventPhotosAsync(eventId);
            if (!result.Success || result.Data == null)
                return NotFound(new BaseResponse<bool> { Success = false, Message = result.Message });

            return Ok(new BaseResponse<List<FileDownloadDto>>
            {
                Success = true,
                Data = result.Data,
            });
        }
    }
}
