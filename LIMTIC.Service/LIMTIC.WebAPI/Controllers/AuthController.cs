using FluentValidation;
using LIMTIC.Application.Commands.Login;
using LIMTIC.Application.Contracts.Auth;
using LIMTIC.Application.Services.Auth;
using LIMTIC.WebAPI.Helpers;
using LIMTIC.WebAPI.Models.Auth.Login;
using LIMTIC.WebAPI.Models.Auth.Logout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMTIC.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest loginRequest,
            [FromServices] IValidator<LoginCommand> validator)
        {
            var loginCommand = new LoginCommand(loginRequest.Username.ToLower(), loginRequest.Password);

            var validationResult = validator.Validate(loginCommand);
            if (!validationResult.IsValid)
            {
                return BadRequest(new LoginResponse
                {
                    Message = "Validation failed",
                    ValidationErrors = ValidationHelper.ParseValidationErrors(validationResult)
                });
            }

            int refreshTokenExpirationDays = _configuration.GetValue<int>("RefreshToken:ExpireInDays");

            var result = await _authService.Login(loginCommand, refreshTokenExpirationDays);
            if (result.IsFailure)
                return BadRequest(new LoginResponse { Message = result.Error });

            Response.Cookies.Append("refreshToken", result.Data!.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
            });

            return Ok(new LoginResponse { AccessToken = result.Data!.AccessToken });
        }

        [HttpPost]
        [Authorize]
        [Route("refresh")]
        public async Task<IActionResult> Refresh()
        {
            Request.Cookies.TryGetValue("refreshToken", out string? refreshToken);

            var result = await _authService.ValidateRefreshToken(refreshToken);
            if (result.IsFailure)
            {
                await _authService.Logout();
                return Unauthorized(new LoginResponse { Message = result.Error });
            }

            return Ok(new LoginResponse { AccessToken = result.Data!.AccessToken });
        }

        [HttpPost]
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout();

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new LogoutResponse { Success = true });
        }
    }
}
