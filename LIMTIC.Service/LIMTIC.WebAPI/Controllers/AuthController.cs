using LIMTIC.Application.Abstractions.Auth;
using LIMTIC.Application.Contracts.Commands.Login;
using LIMTIC.Application.Settings;
using LIMTIC.WebAPI.Models.Auth.Login;
using LIMTIC.WebAPI.Models.Auth.Logout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LIMTIC.WebAPI.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly RefreshTokenSettings _refreshTokenSettings;

        public AuthController(IAuthService authService, IOptions<RefreshTokenSettings> refreshTokenSettings)
        {
            _authService = authService;
            _refreshTokenSettings = refreshTokenSettings.Value;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var loginCommand = new LoginCommand(loginRequest.Username.ToLower(), loginRequest.Password);

            var result = await _authService.Login(loginCommand);
            if (result.IsFailure)
                return BadRequest(new LoginResponse { Message = result.Message, ValidationErrors = result.ValidationErrors });

            Response.Cookies.Append("refreshToken", result.Data!.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(_refreshTokenSettings.ExpireInDays)
                    .AddSeconds(_refreshTokenSettings.ExpireInSeconds)
            });

            return Ok(new LoginResponse { AccessToken = result.Data!.AccessToken });
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh()
        {
            Request.Cookies.TryGetValue("refreshToken", out string? refreshToken);

            var result = await _authService.ValidateRefreshToken(refreshToken);
            if (result.IsFailure)
                return Unauthorized(new LoginResponse { Message = result.Message });

            return Ok(new LoginResponse { AccessToken = result.Data!.AccessToken });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            Request.Cookies.TryGetValue("refreshToken", out string? refreshToken);

            await _authService.Logout(refreshToken);

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.Strict
            });

            return Ok(new LogoutResponse { Success = true });
        }
    }
}
