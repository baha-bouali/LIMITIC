using LIMTIC.Application.Abstractions.UserManagement;
using LIMTIC.Application.Contracts.Commands.ChangeUserPassword;
using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.WebAPI.Base;
using LIMTIC.WebAPI.Mappers.UserMapper;
using LIMTIC.WebAPI.Models.UserManagement.ChangeUserPassword;
using LIMTIC.WebAPI.Models.UserManagement.CreateUser;
using LIMTIC.WebAPI.Models.UserManagement.GetUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers
{
    [ApiController]
    [Route("api/users/")]
    public class UsersManagementController : ControllerBase
    {
        private readonly IUsersManagementService _usersManagementService;
        private readonly IUserMapper _userMapper;

        public UsersManagementController(IUsersManagementService usersManagementService, IUserMapper userMapper)
        {
            _usersManagementService = usersManagementService;
            _userMapper = userMapper;
        }

        [HttpPost("addUser/")]
        [Authorize(Roles = "Super_Admin")]
        public async Task<IActionResult> AddUser(CreateUserRequest user)
        {
            var command = new CreateUserCommand
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                Role = user.Role,
                IsActive = true,
            };

            var result = await _usersManagementService.CreateUserAsync(command);
            if (result.Success)
            {
                return Ok(new CreateUserResponse
                {
                    User = _userMapper.MapToUserDto(result.Data.User)
                });
            }
            else
            {
                return BadRequest(new CreateUserResponse
                {
                    Message = result.Error,
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
                    User = _userMapper.MapToUserDto(result.Data.User)
                });
            }
            else
            {
                return BadRequest(new GetUserResponse
                {
                    Message = result.Error
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
                return Ok();
            }
            else
            {
                return BadRequest(new BaseResponse
                {
                    Message = result.Error,
                    ValidationErrors = result.ValidationErrors
                });
            }
        }
    }
}
