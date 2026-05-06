using LIMTIC.Application.Abstractions.UserManagement;
using LIMTIC.Application.Contracts.Commands.CreateUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LIMTIC.WebAPI.Models;
using LIMTIC.WebAPI.Models.UserManagement.CreateUser;
using LIMTIC.WebAPI.Models.UserManagement.GetUser;

namespace LIMTIC.WebAPI.Controllers
{
    [ApiController]
    [Route("api/users/")]
    public class UsersManagementController : ControllerBase
    {
        private readonly IUsersManagementService _usersManagementService;

        public UsersManagementController(IUsersManagementService usersManagementService)    
        {
            _usersManagementService = usersManagementService;
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
                Role = createUserRequest.Role,
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
                    Success = false
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
                    Success = false
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
                    User = result.Data.User
                });
            }
            else
            {
                return BadRequest(new GetUserResponse
                {
                    Message = result.Message
                });
            }
        }
	}
}
