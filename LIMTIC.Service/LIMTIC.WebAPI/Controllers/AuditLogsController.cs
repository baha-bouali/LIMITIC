using LIMTIC.Application.Abstractions.AuditLogs;
using LIMTIC.WebAPI.Models.AuditLogs.GetAuditLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers
{
    [ApiController]
    [Route("api/auditLogs/")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogsService _auditLogsService;

        public AuditLogsController(IAuditLogsService auditLogsService)
        {
            _auditLogsService = auditLogsService;
        }

        [HttpGet("")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAuditLogs([FromQuery] DateTime fromUtc, [FromQuery] DateTime? toUtc = null)
        {
            var result = await _auditLogsService.GetAuditLogsByPeriodAsync(fromUtc, toUtc);
            var rightBound = toUtc ?? DateTime.UtcNow;

            if (result.Success)
            {
                return Ok(new GetAuditLogsResponse
                {
                    Success = true,
                    Items = result.Data,
                    FromUtc = fromUtc,
                    ToUtc = rightBound
                });
            }

            return BadRequest(new GetAuditLogsResponse
            {
                Success = false,
                Message = result.Message,
                ValidationErrors = result.ValidationErrors,
                FromUtc = fromUtc,
                ToUtc = rightBound
            });
        }
    }
}
