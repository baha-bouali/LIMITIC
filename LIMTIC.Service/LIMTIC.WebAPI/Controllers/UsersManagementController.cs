using LIMTIC.Application.Services;
using LIMTIC.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers
{
    [ApiController]
    [Route("users/")]
    public class UsersManagementController : Controller
    {
        private readonly UsersManagementService UsersManagementService;
        public UsersManagementController(UsersManagementService usersManagementService) 
        {
            UsersManagementService = usersManagementService;
        }

        [HttpPost("addUser/")]
        public async Task<IActionResult> AddUser(User user)
        {
            var result = await UsersManagementService.AddUser(user);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("getUser")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var result = await UsersManagementService.GetUser(id);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
