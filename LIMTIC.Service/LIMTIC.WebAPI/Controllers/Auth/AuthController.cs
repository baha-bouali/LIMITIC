using LIMTIC.Application.Abstractions.Auth;
using LIMTIC.Application.Contracts.Commands.ForgetPassword;
using LIMTIC.Application.Contracts.Commands.Login;
using LIMTIC.Application.Contracts.Commands.ResetPassword;
using LIMTIC.Application.Contracts.Commands.VerifyResetCode;
using LIMTIC.Application.Settings;
using LIMTIC.WebAPI.Models.Auth.ForgetPassword;
using LIMTIC.WebAPI.Models.Auth.Login;
using LIMTIC.WebAPI.Models.Auth.ResetPassword;
using LIMTIC.WebAPI.Models.Auth.VerifyResetCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LIMTIC.WebAPI.Controllers.Auth
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

            return Ok(new LoginResponse { AccessToken = result.Data!.AccessToken, UserId = result.Data!.UserId, Email = result.Data!.Email });
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            Request.Cookies.TryGetValue("refreshToken", out string? refreshToken);

            var result = await _authService.ValidateRefreshToken(refreshToken);
            if (result.IsFailure)
                return Unauthorized(new LoginResponse { Message = result.Message });

            return Ok(new LoginResponse { AccessToken = result.Data!.AccessToken, UserId = result.Data!.UserId, Email = result.Data!.Email });
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

            return Ok(new BaseResponse { Success = true });
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordRequest request)
        {
            var command = new ForgetPasswordCommand
            {
                email = request.Email
            };
            var result = await _authService.ForgetPasswordAsync(command);
            if (result.Success)
                return Ok(new BaseResponse { Message = result.Data });
            else
                return BadRequest(new BaseResponse { Message = result.Message });
        }

        [HttpPost("verifyOTP")]
        public async Task<IActionResult> VerifyOTP(VerifyResetCodeRequest request)
        {
            var command = new VerifyResetCodeCommand
            {
                Email = request.Email,
                OtpToken = request.OtpToken
            };
            var result = await _authService.VerifyResetTokenAsync(command);
            if (result.Success)
            {
                return Ok(new VerifyResetCodeResponse
                {
                    ResetToken = result.Data.ResetToken,
                });
            }
            else
            {
                return BadRequest(new BaseResponse
                {
                    Message = result.Message
                });
            }
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var command = new ResetPasswordCommand
            {
                Email = request.Email,
                NewPassword = request.NewPassword,
                ResetToken = request.ResetToken
            };
            var result = await _authService.ResetPasswordAsync(command);
            if (result.Success)
            {
                return Ok(new BaseResponse
                {
                    Message = "Password reset successful"
                });
            }
            else
            {
                return BadRequest(new BaseResponse
                {
                    Message = result.Message
                });
            }
        }
    }
}

