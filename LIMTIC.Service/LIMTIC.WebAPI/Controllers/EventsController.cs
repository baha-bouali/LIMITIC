using LIMTIC.Application.Abstractions.Events;
using LIMTIC.Application.DTOs.Events;
using LIMTIC.WebAPI.Models.Events.GetEvents;
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
    }
}
