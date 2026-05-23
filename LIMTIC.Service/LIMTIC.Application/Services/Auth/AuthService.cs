using FluentValidation;
using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Auth;
using LIMTIC.Application.Abstractions.Email;
using LIMTIC.Application.Abstractions.Security;
using LIMTIC.Application.Contracts.Commands.ForgetPassword;
using LIMTIC.Application.Contracts.Commands.Login;
using LIMTIC.Application.Contracts.Commands.ResetPassword;
using LIMTIC.Application.Contracts.Commands.VerifyResetCode;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.Emails.Models;
using LIMTIC.Application.Helpers;
using LIMTIC.Application.Interfaces.Services;
using LIMTIC.Application.Settings;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.RefreshToken;
using LIMTIC.Domain.Entities.ResetPassword;
using LIMTIC.Domain.Entities.Users;
using Microsoft.Extensions.Options;

namespace LIMTIC.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IResetPasswordRepository _resetPasswordRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly RefreshTokenSettings _refreshTokenSettings;
        private readonly OTPTokenSettings _otpTokenSettings;
        private readonly ResetPasswordTokenSettings _resetPasswordTokenSettings;
        private readonly IValidator<LoginCommand> _loginCommandValidator;

        public AuthService(
            ITokenService tokenService,
            ICurrentUserService currentUserService,
            IEmailService emailService,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IResetPasswordRepository resetPasswordRepository,
            IPasswordHasher passwordHasher,
            IOptions<RefreshTokenSettings> refreshTokenSettings,
            IOptions<OTPTokenSettings> otpTokenSettings,
            IOptions<ResetPasswordTokenSettings> resetPasswordTokenSettings,
            IValidator<LoginCommand> loginCommandValidator)
        {
            _tokenService = tokenService;
            _currentUserService = currentUserService;
            _emailService = emailService;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _resetPasswordRepository = resetPasswordRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenSettings = refreshTokenSettings.Value;
            _otpTokenSettings = otpTokenSettings.Value;
            _resetPasswordTokenSettings = resetPasswordTokenSettings.Value;
            _loginCommandValidator = loginCommandValidator;
        }

        public async Task<Result<LoginCommandResponse>> Login(LoginCommand command)
        {
            var validationResult = _loginCommandValidator.Validate(command);
            if (!validationResult.IsValid)
                return Result<LoginCommandResponse>.ValidationFailureResult(ValidationHelper.ParseValidationErrors(validationResult));

            var user = await _userRepository.GetUserByEmailAsync(command.Username);
            if (user == null)
                return Result<LoginCommandResponse>.FailureResult("Invalid username");

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, command.Password))
                return Result<LoginCommandResponse>.FailureResult("Invalid password");

            string accessToken = _tokenService.GenerateAccessToken(user);
            string refreshToken = _tokenService.GenerateToken();

            await _refreshTokenRepository.AddRefreshTokenAsync(new RefreshTokenEntity
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(_refreshTokenSettings.ExpireInDays)
                    .AddSeconds(_refreshTokenSettings.ExpireInSeconds)
            });

            return Result<LoginCommandResponse>.SuccessResult(new LoginCommandResponse(accessToken, refreshToken));
        }

        public async Task<Result<LoginCommandResponse>> ValidateRefreshToken(string? refreshToken)
        {
            if (refreshToken == null)
                return Result<LoginCommandResponse>.FailureResult("Refresh token not found in cookie");

            var token = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);
            if (token == null)
                return Result<LoginCommandResponse>.FailureResult("Refresh token not found");

            if (token.ExpiryDate < DateTime.UtcNow)
                return Result<LoginCommandResponse>.FailureResult("Refresh token is expired");

            string accessToken = _tokenService.GenerateAccessToken(token.User!);

            return Result<LoginCommandResponse>.SuccessResult(new LoginCommandResponse(accessToken, refreshToken));
        }

        public async Task Logout(string? refreshToken)
        {
            await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken);
        }

        public async Task<Result<string>> ForgetPasswordAsync(ForgetPasswordCommand command)
        {
            var user = await _userRepository.GetUserByEmailAsync(command.email);
            if (user == null)
                return Result<string>.FailureResult("User not found");

            var otp = _tokenService.GenerateOTPToken();
            await upsertResetPassword(user, otp);

            await _emailService.SendOTPEmailAsync(new OTPEmailModel
            {
                ToEmail = user.Email,
                OTPCode = otp,
                ExpiryMinutes = _otpTokenSettings.ExpireInMinutes
            });

            return Result<string>.SuccessResult("OTP sent to email");
        }

        public async Task<Result<VerifyResetCodeCommandResponse>> VerifyResetTokenAsync(VerifyResetCodeCommand command)
        {
            var user = await _userRepository.GetUserByEmailAsync(command.Email);
            if (user == null)
                return Result<VerifyResetCodeCommandResponse>.FailureResult("User not found");

            var resetPasswordEntry = await _resetPasswordRepository.GetResetPasswordAsync(user.Id);
            if (resetPasswordEntry == null)
                return Result<VerifyResetCodeCommandResponse>.FailureResult("No reset token found");

            if (!_passwordHasher.VerifyPassword(resetPasswordEntry.OTPTokenHash, command.OtpToken))
                return Result<VerifyResetCodeCommandResponse>.FailureResult("Invalid OTP token");

            if (resetPasswordEntry.OTPTokenExpiry < DateTime.UtcNow)
                return Result<VerifyResetCodeCommandResponse>.FailureResult("OTP token is expired");

            var resetToken = _tokenService.GenerateToken();
            resetPasswordEntry.ResetPasswordTokenHash = _passwordHasher.HashPassword(resetToken);
            resetPasswordEntry.ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(_resetPasswordTokenSettings.ExpireInMinutes);

            await _resetPasswordRepository.UpdateResetPasswordAsync(resetPasswordEntry);

            return Result<VerifyResetCodeCommandResponse>.SuccessResult(new VerifyResetCodeCommandResponse
            {
                ResetToken = resetToken
            });
        }

        public async Task<Result<string>> ResetPasswordAsync(ResetPasswordCommand command)
        {
            var user = await _userRepository.GetUserByEmailAsync(command.Email);
            if (user == null)
                return Result<string>.FailureResult("User not found");

            var resetPasswordEntry = await _resetPasswordRepository.GetResetPasswordAsync(user.Id);
            if (resetPasswordEntry == null)
                return Result<string>.FailureResult("No reset token found");

            if (resetPasswordEntry.ResetPasswordTokenHash == null || command.ResetToken == null)
                return Result<string>.FailureResult("Invalid reset token");

            if (!_passwordHasher.VerifyPassword(resetPasswordEntry.ResetPasswordTokenHash, command.ResetToken))
                return Result<string>.FailureResult("Invalid reset token");

            if (resetPasswordEntry.ResetPasswordTokenExpiry < DateTime.UtcNow)
                return Result<string>.FailureResult("Reset token is expired");

            user.PasswordHash = _passwordHasher.HashPassword(command.NewPassword);
            var updateResult = await _userRepository.UpdateUserAsync(user);
            if (!updateResult)
                return Result<string>.FailureResult("Failed to reset password");

            return Result<string>.SuccessResult("Password reset successful");
        }

        private async Task upsertResetPassword(UserEntity user, string otp)
        {
            var existing = await _resetPasswordRepository.GetResetPasswordAsync(user.Id);

            if (existing != null)
            {
                existing.OTPTokenHash = _passwordHasher.HashPassword(otp);
                existing.OTPTokenExpiry = DateTime.UtcNow.AddMinutes(_otpTokenSettings.ExpireInMinutes);
                existing.ResetPasswordTokenHash = null;
                existing.ResetPasswordTokenExpiry = null;
                await _resetPasswordRepository.UpdateResetPasswordAsync(existing);
            }
            else
            {
                var resetPassword = new ResetPasswordEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    OTPTokenHash = _passwordHasher.HashPassword(otp),
                    OTPTokenExpiry = DateTime.UtcNow.AddMinutes(_otpTokenSettings.ExpireInMinutes)
                };

                await _resetPasswordRepository.AddResetPasswordAsync(resetPassword);
            }
        }
    }
}
