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
    public class UsersManagementController : Controller
    {
        private readonly IUsersManagementService UsersManagementService;
        private readonly IUserMapper _userMapper;

        public UsersManagementController(IUsersManagementService usersManagementService, IUserMapper userMapper)    
        {
            UsersManagementService = usersManagementService;
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

            var result = await UsersManagementService.CreateUserAsync(command);
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
            var result = await UsersManagementService.GetUserByIdAsync(id);
            if (result != null)
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
