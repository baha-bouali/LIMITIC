using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.UserManagement;
using LIMTIC.Application.Contracts.Commands.ChangeUserPassword;
using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Commands.GetUser;
using LIMTIC.Application.Contracts.Commands.UpdateUserRole;
using LIMTIC.Application.DTOs.UserManagement.GetUser;
using LIMTIC.Domain.Enums;
using LIMTIC.WebAPI.Models.UserManagement.ChangeUserPassword;
using LIMTIC.WebAPI.Models.UserManagement.CreateUser;
using LIMTIC.WebAPI.Models.UserManagement.GetUsers;
using LIMTIC.WebAPI.Models.UserManagement.UpdateUserRole;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers
{
    [ApiController]
    [Route("api/users/")]
    public class UsersManagementController : ControllerBase
    {
        private readonly IUsersManagementService _usersManagementService;
        private readonly ICurrentUserService _currentUserService;

        public UsersManagementController(
            IUsersManagementService usersManagementService,
            ICurrentUserService currentUserService)
        {
            _usersManagementService = usersManagementService;
            _currentUserService = currentUserService;
        }

        // ── GET list ───────────────────────────────────────────────────────────────

        /// <summary>
        /// List users with optional filters.
        /// Query params: role, status (active|inactive), q (search firstName/lastName/email), page, limit
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] UserRole? role,
            [FromQuery] string? status,
            [FromQuery] string? q,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            bool? isActive = status switch
            {
                "active"   => true,
                "inactive" => false,
                _          => null
            };

            var result = await _usersManagementService.GetUsersAsync(role, isActive, q, page, limit);

            var counts = result.Data!.Counts.ToDictionary(
                kvp => kvp.Key switch
                {
                    UserRole.SuperAdmin => "SUPER_ADMIN",
                    UserRole.Admin      => "ADMIN",
                    UserRole.Researcher => "CHERCHEUR",
                    UserRole.PhDStudent => "DOCTORANT",
                    UserRole.Masterian  => "MASTERIEN",
                    UserRole.Visitor    => "VISITEUR",
                    _                   => kvp.Key.ToString().ToUpper()
                },
                kvp => kvp.Value);

            return Ok(new GetUsersResponse
            {
                Success = true,
                Items   = result.Data.Items,
                Total   = result.Data.Total,
                Counts  = counts,
                Page    = result.Data.Page,
                Limit   = result.Data.Limit
            });
        }

        [HttpPost("addUser/")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AddUser(CreateUserRequest createUserRequest)
        {
            var command = new CreateUserCommand
            {
                FirstName = createUserRequest.FirstName,
                LastName = createUserRequest.LastName,
                Email = createUserRequest.Email,
                Password = createUserRequest.Password,
                IsActive = createUserRequest.IsActive,
            };

            var result = await _usersManagementService.CreateUserAsync(command);
            if (result.Success)
            {
                return Ok(new CreateUserResponse
                {
                    Success = true,
                    User = result.Data.User
                });
            }
            else
            {
                return BadRequest(new CreateUserResponse
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });
            }
        }

        [HttpPut("updateRole/{userId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> UpdateUserRole(Guid userId, UpdateUserRoleRequest request)
        {
            var command = new UpdateUserRoleCommand
            {
                UserId = userId,
                Role = request.Role,
                Rank = request.Rank,
                Specialty = request.Specialty,
                Office = request.Office,
                PhoneNumber = request.PhoneNumber,
                ResearchAxisIds = request.ResearchAxisIds,
                EnrollmentYear = request.EnrollmentYear,
                Cohort = request.Cohort,
                DissertationSubject = request.DissertationSubject
            };

            var result = await _usersManagementService.UpdateUserRoleAsync(command);
            if (result.Success)
            {
                return Ok(new BaseResponse { Success = true });
            }
            else
            {
                return BadRequest(new BaseResponse { Success = false, Message = result.Message, ValidationErrors = result.ValidationErrors });
            }
        }

        [HttpPost("activateUser/")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ActivateUser(Guid userId)
        {
            var result = await _usersManagementService.ActivateUserAsync(userId);
            if (result.Success)
            {
                return Ok(new BaseResponse
                {
                    Success = true,
                });
            }
            else
            {
                return BadRequest(new BaseResponse
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });
            }
        }

        [HttpPost("deactivateUser/")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeactivateUser(Guid userId)
        {
            var result = await _usersManagementService.DeactivateUserAsync(userId);
            if (result.Success)
            {
                return Ok(new BaseResponse
                {
                    Success = true,
                });
            }
            else
            {
                return BadRequest(new BaseResponse
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });
            }
        }

        [HttpGet("getUser")]
        [Authorize]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var result = await _usersManagementService.GetUserByIdAsync(id);
            if (result.Success)
            {
                return Ok(new GetUserResponse
                {
                    Success = true,
                    User = result.Data.User
                });
            }
            else
            {
                return BadRequest(new GetUserResponse
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });
            }
        }

        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangeUserPasswordRequest request)
        {
            var command = new ChangeUserPasswordCommand
            {
                Email = request.Email,
                OldPassword = request.OldPassword,
                NewPassword = request.NewPassword
            };
            var result = await _usersManagementService.ChangeUserPasswordAsync(command);
            if (result.Success)
            {
                return Ok(new BaseResponse
                {
                    Success = true,
                });
            }
            else
            {
                return BadRequest(new BaseResponse
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });
            }
        }

        [HttpPost("{userId:guid}/avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(Guid userId, IFormFile avatar)
        {
            var isAdmin = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
            if (!isAdmin && _currentUserService.UserId != userId)
                return Forbid();

            if (avatar == null || avatar.Length == 0)
                return BadRequest(new BaseResponse { Success = false, Message = "No file provided" });

            await using var stream = avatar.OpenReadStream();
            var result = await _usersManagementService.UpdateUserAvatarAsync(
                userId, stream, avatar.FileName, avatar.ContentType);

            if (result.Success)
                return Ok(new BaseResponse { Success = true, Message = result.Data });

            return BadRequest(new BaseResponse { Success = false, Message = result.Message });
        }

        [HttpGet("{userId:guid}/avatar")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvatar(Guid userId)
        {
            var result = await _usersManagementService.GetUserAvatarAsync(userId);
            if (!result.Success || result.Data == null)
                return NotFound(new BaseResponse { Success = false, Message = result.Message });

            return File(result.Data.Stream, result.Data.ContentType, result.Data.FileName);
        }
    }
}
