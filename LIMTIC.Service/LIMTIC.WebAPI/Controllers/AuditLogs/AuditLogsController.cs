using LIMTIC.Application.Abstractions.AuditLogs;
using LIMTIC.Application.Contracts.Queries.AuditLogs;
using LIMTIC.Application.DTOs.AuditLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers.AuditLogs
{
    [ApiController]
    [Route("api/auditLogs/")]
    [Authorize(Roles = "SuperAdmin")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogsService _auditLogsService;

        public AuditLogsController(IAuditLogsService auditLogsService)
        {
            _auditLogsService = auditLogsService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAuditLogs(GetAuditLogsQuery getAuditLogsQuery)
        {
            var result = await _auditLogsService.GetAuditLogsByPeriodAsync(getAuditLogsQuery);
            var rightBound = getAuditLogsQuery.ToUtc ?? DateTime.UtcNow;

            if (result.Success)
            {
                return Ok(new BaseResponse<List<AuditLogDto>>
                {
                    Success = true,
                    Data = result.Data,
                });
            }

            return BadRequest(new BaseResponse<List<AuditLogDto>>
            {
                Success = false,
                Message = result.Message,
                ValidationErrors = result.ValidationErrors
            });
        }
    }
}
