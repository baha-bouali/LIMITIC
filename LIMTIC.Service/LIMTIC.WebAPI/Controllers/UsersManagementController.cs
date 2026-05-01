using LIMTIC.Application.Commands.CreateUser;
using LIMTIC.Application.Contracts.UserManagement;
using LIMTIC.WebAPI.Mappers.UserMapper;
using LIMTIC.WebAPI.Models.UserManagement.CreateUser;
using LIMTIC.WebAPI.Models.UserManagement.GetUser;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers
{
    [ApiController]
    [Route("users/")]
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
        public async Task<IActionResult> AddUser(CreateUserRequest user)
        {
            var command = new CreateUserCommand
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
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
                    Message = result.Error
                });
            }
        }

        [HttpGet("getUser")]
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
    }
}
