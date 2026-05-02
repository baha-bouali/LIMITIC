using LIMTIC.Application.Commands.Login;
using LIMTIC.Application.Contracts.Auth;
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
                return BadRequest(new LoginResponse { Message = result.Error, ValidationErrors = result.ValidationErrors });

            Response.Cookies.Append("refreshToken", result.Data!.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(_refreshTokenSettings.ExpireInDays),
            });

            return Ok(new LoginResponse { AccessToken = result.Data!.AccessToken });
        }

        [HttpPost("refresh")]
        [Authorize]
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

        [HttpPost("logout")]
        [Authorize]
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
