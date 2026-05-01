using FluentValidation;
using LIMTIC.Application.DTOs.Auth;
using LIMTIC.Application.Services.Auth;
using LIMTIC.WebAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LIMTIC.WebAPI.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest loginRequest,
            [FromServices] IValidator<LoginRequest> validator)
        {
            var validationResult = validator.Validate(loginRequest);
            if (!validationResult.IsValid)
                return BadRequest(ValidationHelper.ParseValidationErrors(validationResult));

            int refreshTokenExpirationDays = _configuration.GetValue<int>("RefreshToken:ExpireInDays");

            var result = await _authService.Login(loginRequest, refreshTokenExpirationDays);
            if (result.IsFailure)
                return BadRequest(new { error = result.ErrorMessage });

            Response.Cookies.Append("refreshToken", result.Value!.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
            });

            return Ok(new { result.Value!.AccessToken });
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
                return Unauthorized(result.ErrorMessage);
            }

            return Ok(new { result.Value!.AccessToken });
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

            return NoContent();
        }
    }
}
