using FluentValidation;
using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Security;
using LIMTIC.Application.Contracts.Commands.Login;
using LIMTIC.Application.Helpers;
using LIMTIC.Application.Settings;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities;
using LIMTIC.Domain.Shared;
using Microsoft.Extensions.Options;
using LIMTIC.Application.Abstractions.Auth;
using LIMTIC.Domain.Entities.RefreshToken;

namespace LIMTIC.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly RefreshTokenSettings _refreshTokenSettings;
        private readonly IValidator<LoginCommand> _loginCommandValidator;

        public AuthService(
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            ICurrentUserService currentUserService,
            IOptions<RefreshTokenSettings> refreshTokenSettings,
            IValidator<LoginCommand> loginCommandValidator)
        {
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _currentUserService = currentUserService;
            _refreshTokenSettings = refreshTokenSettings.Value;
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
            string refreshToken = _tokenService.GenerateRefreshToken();

            await _refreshTokenRepository.AddRefreshTokenAsync(new RefreshTokenEntity
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(_refreshTokenSettings.ExpireInDays)
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

            if (token.ExpiryDate > DateTime.UtcNow)
                return Result<LoginCommandResponse>.FailureResult("Refresh token is expired");

            string accessToken = _tokenService.GenerateAccessToken(token.User!);

            return Result<LoginCommandResponse>.SuccessResult(new LoginCommandResponse(accessToken, refreshToken));
        }

        public async Task Logout()
        {
            var currentUserId = _currentUserService.UserId;
            await _refreshTokenRepository.RevokeRefreshTokenAsync(currentUserId);
        }
    }
}
