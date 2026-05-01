using LIMTIC.Application.Abstractions;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Application.Abstractions.Security;
using LIMTIC.Application.DTOs.Auth;
using LIMTIC.Domain.Entities;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Application.Services.Auth
{
    public class AuthService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICurrentUserService _currentUserService;

        public AuthService(
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            ICurrentUserService currentUserService)
        {
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ResultT<LoginResponse>> Login(LoginRequest loginRequest, int refreshTokenExpirationDays)
        {
            var user = await _userRepository.GetUserByEmail(loginRequest.Username);
            if (user == null)
                return ResultT<LoginResponse>.Failure("Invalid username");

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, loginRequest.Password))
                return ResultT<LoginResponse>.Failure("Invalid password");

            string accessToken = _tokenService.GenerateAccessToken(user);
            string refreshToken = _tokenService.GenerateRefreshToken();

            await _refreshTokenRepository.AddRefreshTokenAsync(new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(refreshTokenExpirationDays)
            });

            return ResultT<LoginResponse>.Success(new LoginResponse(accessToken, refreshToken));
        }

        public async Task<ResultT<LoginResponse>> ValidateRefreshToken(string? refreshToken)
        {
            if (refreshToken == null)
                return ResultT<LoginResponse>.Failure("Refresh token not found in cookie");

            var token = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);
            if (token == null)
                return ResultT<LoginResponse>.Failure("Refresh token not found");

            if (token.ExpiryDate > DateTime.UtcNow)
                return ResultT<LoginResponse>.Failure("Refresh token is expired");

            string accessToken = _tokenService.GenerateAccessToken(token.User!);

            return ResultT<LoginResponse>.Success(new LoginResponse(accessToken, refreshToken));
        }

        public async Task Logout()
        {
            var currentUserId = _currentUserService.UserId;
            await _refreshTokenRepository.RevokeRefreshToken(currentUserId);
        }
    }
}
