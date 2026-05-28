using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.UserManagement;
using LIMTIC.Application.Contracts.Commands.ChangeUserPassword;
using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Commands.UpdateUserRole;
using LIMTIC.Application.Contracts.Queries.Users;
using LIMTIC.Application.DTOs.Storage;
using LIMTIC.Application.DTOs.UserManagement;
using LIMTIC.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers.Users
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
        [AllowAnonymous]
        public async Task<IActionResult> GetUsers(GetUsersQuery getUsersQuery)
        {
            var result = await _usersManagementService.GetUsersAsync(getUsersQuery);

            return Ok(new BaseResponse<GetUsersResult>
            {
                Success = true,
                Data = result.Data,
                Pagination = new PaginationResponse
                {
                    Total = result.Data.Total,
                    Page = getUsersQuery.Page,
                    Limit = getUsersQuery.Limit
                }
            });
        }

        [HttpPost("addUser/")]
        [AllowAnonymous]
        public async Task<IActionResult> AddUser(CreateUserCommand command)
        {
            var result = await _usersManagementService.CreateUserAsync(command);
            if (result.Success)
            {
                return Ok(new BaseResponse<UserDto>
                {
                    Success = true,
                    Data = result.Data
                });
            }
            else
            {
                return BadRequest(new BaseResponse<UserDto>
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });
            }
        }

        [HttpPut("updateRole/{userId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> UpdateUserRole(UpdateUserRoleCommand command)
        {
            var result = await _usersManagementService.UpdateUserRoleAsync(command);
            if (result.Success)
            {
                return Ok(new BaseResponse<bool> { Success = true });
            }
            else
            {
                return BadRequest(new BaseResponse<bool> { Success = false, Message = result.Message, ValidationErrors = result.ValidationErrors });
            }
        }

        [HttpPost("activateUser/")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ActivateUser(Guid userId)
        {
            var result = await _usersManagementService.ActivateUserAsync(userId);
            if (result.Success)
            {
                return Ok(new BaseResponse<bool>
                {
                    Success = true,
                });
            }
            else
            {
                return BadRequest(new BaseResponse<bool>
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });
            }
        }

        [HttpPost("deactivateUser/")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> DeactivateUser(Guid userId)
        {
            var result = await _usersManagementService.DeactivateUserAsync(userId);
            if (result.Success)
            {
                return Ok(new BaseResponse<bool>
                {
                    Success = true,
                });
            }
            else
            {
                return BadRequest(new BaseResponse<bool>
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
                return Ok(new BaseResponse<UserDto>
                {
                    Success = true,
                    Data = result.Data
                });
            }
            else
            {
                return BadRequest(new BaseResponse<UserDto>
                {
                    Success = false,
                    Message = result.Message,
                    ValidationErrors = result.ValidationErrors
                });
            }
        }

        [HttpDelete("{userId:guid}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var result = await _usersManagementService.DeleteUserAsync(userId);
            if (result.Success)
                return Ok(new BaseResponse<bool> { Success = true });

            return result.Message == "User not found"
                ? NotFound(new BaseResponse<bool> { Success = false, Message = result.Message })
                : BadRequest(new BaseResponse<bool> { Success = false, Message = result.Message });
        }

        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangeUserPasswordCommand command)
        {
            var result = await _usersManagementService.ChangeUserPasswordAsync(command);
            if (result.Success)
            {
                return Ok(new BaseResponse<bool>
                {
                    Success = true,
                });
            }
            else
            {
                return BadRequest(new BaseResponse<bool>
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
            if (_currentUserService.UserId != userId)
                return Forbid();

            if (avatar == null || avatar.Length == 0)
                return BadRequest(new BaseResponse<bool> { Success = false, Message = "No file provided" });

            await using var stream = avatar.OpenReadStream();
            var result = await _usersManagementService.UpdateUserAvatarAsync(
                userId, stream, avatar.FileName, avatar.ContentType);

            if (result.Success)
                return Ok(new BaseResponse<bool> { Success = true, Message = result.Data });

            return BadRequest(new BaseResponse<bool> { Success = false, Message = result.Message });
        }

        [HttpGet("{userId:guid}/avatar")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvatar(Guid userId)
        {
            var result = await _usersManagementService.GetUserAvatarAsync(userId);
            if (!result.Success || result.Data == null)
                return NotFound(new BaseResponse<bool> { Success = false, Message = result.Message });

            return Ok(new BaseResponse<FileDownloadDto>
            {
                Success = true,
                Data = result.Data
            });
        }
    }
}
