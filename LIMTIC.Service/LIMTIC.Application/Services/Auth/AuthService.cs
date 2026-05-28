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
using LIMTIC.Application.DTOs.Auth;
using LIMTIC.Application.Emails.Models;
using LIMTIC.Application.Helpers;
using LIMTIC.Application.Settings;
using LIMTIC.Domain.Abstractions.AuditLogs;
using LIMTIC.Domain.Abstractions.Users;
using LIMTIC.Domain.Entities.RefreshToken;
using LIMTIC.Domain.Entities.ResetPassword;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;
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
        private readonly IAuditLogsRepository _auditLogsRepository;
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
            IAuditLogsRepository auditLogsRepository,
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
            _auditLogsRepository = auditLogsRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenSettings = refreshTokenSettings.Value;
            _otpTokenSettings = otpTokenSettings.Value;
            _resetPasswordTokenSettings = resetPasswordTokenSettings.Value;
            _loginCommandValidator = loginCommandValidator;
        }

        public async Task<Result<LoginDto>> Login(LoginCommand command)
        {
            var validationResult = _loginCommandValidator.Validate(command);
            if (!validationResult.IsValid)
                return Result<LoginDto>.ValidationFailureResult(ValidationHelper.ParseValidationErrors(validationResult));

            var user = await _userRepository.GetUserByEmailAsync(command.Username);
            if (user == null)
                return Result<LoginDto>.FailureResult("Invalid username");

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, command.Password))
                return Result<LoginDto>.FailureResult("Invalid password");

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

            var loginLog = AuditLogHelper.CreateAuditLog(user.Id, ActionType.LOGIN, ResourceType.User);
            await _auditLogsRepository.AddLog(loginLog);

            return Result<LoginDto>.SuccessResult(new LoginDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken, 
                UserId = user.Id, 
                Email = user.Email
            });
        }

        public async Task<Result<LoginDto>> ValidateRefreshToken(string? refreshToken)
        {
            if (refreshToken == null)
                return Result<LoginDto>.FailureResult("Refresh token not found in cookie");

            var token = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);
            if (token == null)
                return Result<LoginDto>.FailureResult("Refresh token not found");

            if (token.ExpiryDate < DateTime.UtcNow)
                return Result<LoginDto>.FailureResult("Refresh token is expired");

            string accessToken = _tokenService.GenerateAccessToken(token.User!);

            var validateLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId.Value, ActionType.VALIDATE, ResourceType.User);
            await _auditLogsRepository.AddLog(validateLog);

            return Result<LoginDto>.SuccessResult(new LoginDto
            {
                AccessToken = accessToken, 
                RefreshToken = refreshToken, 
                UserId = token.User!.Id, 
                Email = token.User!.Email 
            });
        }

        public async Task<Result<bool>> Logout(string? refreshToken)
        {
            int result = await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken);
            
            var logoutLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId.Value, ActionType.LOGOUT, ResourceType.User);
            await _auditLogsRepository.AddLog(logoutLog);

            return result > 0 ? Result<bool>.SuccessResult(true) : Result<bool>.FailureResult("Failed to revoke refresh token");
        }

        public async Task<Result<string>> ForgetPasswordAsync(ForgetPasswordCommand command)
        {
            var user = await _userRepository.GetUserByEmailAsync(command.Email);
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

            var forgetPasswordLog = AuditLogHelper.CreateAuditLog(user.Id, ActionType.VALIDATE, ResourceType.User);
            await _auditLogsRepository.AddLog(forgetPasswordLog);

            return Result<string>.SuccessResult("OTP sent to email");
        }

        public async Task<Result<string>> VerifyResetTokenAsync(VerifyResetCodeCommand command)
        {
            var user = await _userRepository.GetUserByEmailAsync(command.Email);
            if (user == null)
                return Result<string>.FailureResult("User not found");

            var resetPasswordEntry = await _resetPasswordRepository.GetResetPasswordAsync(user.Id);
            if (resetPasswordEntry == null)
                return Result<string>.FailureResult("No reset token found");

            if (!_passwordHasher.VerifyPassword(resetPasswordEntry.OTPTokenHash, command.OtpToken))
                return Result<string>.FailureResult("Invalid OTP token");

            if (resetPasswordEntry.OTPTokenExpiry < DateTime.UtcNow)
                return Result<string>.FailureResult("OTP token is expired");

            var resetToken = _tokenService.GenerateToken();
            resetPasswordEntry.ResetPasswordTokenHash = _passwordHasher.HashPassword(resetToken);
            resetPasswordEntry.ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(_resetPasswordTokenSettings.ExpireInMinutes);

            await _resetPasswordRepository.UpdateResetPasswordAsync(resetPasswordEntry);

            var verifyResetLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId.Value, ActionType.VALIDATE, ResourceType.User);
            await _auditLogsRepository.AddLog(verifyResetLog);

            return Result<string>.SuccessResult(resetToken);
        }

        public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordCommand command)
        {
            var user = await _userRepository.GetUserByEmailAsync(command.Email);
            if (user == null)
                return Result<bool>.FailureResult("User not found");

            var resetPasswordEntry = await _resetPasswordRepository.GetResetPasswordAsync(user.Id);
            if (resetPasswordEntry == null)
                return Result<bool>.FailureResult("No reset token found");

            if (resetPasswordEntry.ResetPasswordTokenHash == null || command.ResetToken == null)
                return Result<bool>.FailureResult("Invalid reset token");

            if (!_passwordHasher.VerifyPassword(resetPasswordEntry.ResetPasswordTokenHash, command.ResetToken))
                return Result<bool>.FailureResult("Invalid reset token");

            if (resetPasswordEntry.ResetPasswordTokenExpiry < DateTime.UtcNow)
                return Result<bool>.FailureResult("Reset token is expired");

            user.PasswordHash = _passwordHasher.HashPassword(command.NewPassword);
            var updateResult = await _userRepository.UpdateUserAsync(user);
            if (!updateResult)
                return Result<bool>.FailureResult("Failed to reset password");

            var resetPasswordLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId.Value, ActionType.UPDATE, ResourceType.User);
            await _auditLogsRepository.AddLog(resetPasswordLog);

            return Result<bool>.SuccessResult(true);
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
