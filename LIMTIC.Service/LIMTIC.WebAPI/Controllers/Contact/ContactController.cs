using LIMTIC.Application.Abstractions.Contacts;
using LIMTIC.Application.Contracts.Commands.Contacts;
using LIMTIC.WebAPI.Models.Contact;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers.Contact
{
    [ApiController]
    [Route("api/contact")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendContactMessageCommand command)
        {
            var result = await _contactService.SendContactMessageAsync(command);
            if (result.Success)
            {
                return Ok(new BaseResponse<string>
                {
                    Success = true,
                    Data = result.Data
                });
            }

            return BadRequest(new BaseResponse<string>
            {
                Success = false,
                Message = result.Message,
                ValidationErrors = result.ValidationErrors
            });
        }
    }
}