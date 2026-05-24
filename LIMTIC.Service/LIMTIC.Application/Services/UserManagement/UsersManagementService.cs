using FluentValidation;
using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Security;
using LIMTIC.Application.Abstractions.UserManagement;
using LIMTIC.Application.Contracts.Commands.ChangeUserPassword;
using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Commands.GetUser;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.Helpers;
using LIMTIC.Application.Mappers.UserMapper;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Services.UserManagement
{
    public class UsersManagementService : IUsersManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserMapper _userMapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IValidator<CreateUserCommand> _createUserCommandValidator;
        private readonly IAuditLogsRepository _auditLogsRepository;
        private readonly ICurrentUserService _currentUserService;

        public UsersManagementService(
            IUserRepository userRepository,
            IUserMapper userMapper,
            IPasswordHasher passwordHasher,
            IValidator<CreateUserCommand> createUserCommandValidator,
            IAuditLogsRepository auditLogsRepository,
            ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _createUserCommandValidator = createUserCommandValidator;
            _userMapper = userMapper;
            _auditLogsRepository = auditLogsRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<CreateUserCommandResponse>> CreateUserAsync(CreateUserCommand command)
        {
            var validationResult = _createUserCommandValidator.Validate(command);
            if (!validationResult.IsValid)
                return Result<CreateUserCommandResponse>.ValidationFailureResult(ValidationHelper.ParseValidationErrors(validationResult));

            var existingUser = await _userRepository.GetUserByEmailAsync(command.Email);
            if (existingUser != null)
                return Result<CreateUserCommandResponse>.FailureResult("Email already registered");

            var user = UserEntity.Create(
                email: command.Email,
                firstName: command.FirstName,
                lastName: command.LastName,
                passwordHash: _passwordHasher.HashPassword(command.Password),
                isActive: command.IsActive,
                role: command.Role);

            var result = await _userRepository.AddUserAsync(user);
            if (!result)
                return Result<CreateUserCommandResponse>.FailureResult("Failed to create user");

            var log = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.CREATE, ResourceType.User);
            await _auditLogsRepository.AddLog(log);

            return Result<CreateUserCommandResponse>.SuccessResult(new CreateUserCommandResponse
            {
                User = _userMapper.MapToUserDto(user),
            });
        }

        public async Task<Result<GetUserCommandResponse>> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return user != null ? Result<GetUserCommandResponse>.SuccessResult(new GetUserCommandResponse
            {
                User = _userMapper.MapToUserDto(user)
            }) : Result<GetUserCommandResponse>.FailureResult("User not found");
        }

        public async Task<Result<bool>> ActivateUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return Result<bool>.FailureResult("User not found");

            if (user.IsActive)
                return Result<bool>.SuccessResult(data: true, message: "User is already active");

            user.IsActive = true;
            var result = await _userRepository.UpdateUserAsync(user);

            if (!result)
                return Result<bool>.FailureResult("Failed to activate user");

            var log = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.UPDATE, ResourceType.User);
            await _auditLogsRepository.AddLog(log);

            return Result<bool>.SuccessResult(data: true);
        }

        public async Task<Result<bool>> DeactivateUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return Result<bool>.FailureResult("User not found");

            if (!user.IsActive)
                return Result<bool>.FailureResult("User is already deactived");

            user.IsActive = false;
            var result = await _userRepository.UpdateUserAsync(user);

            if (!result)
                return Result<bool>.FailureResult("Failed to activate user");

            var log = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.UPDATE, ResourceType.User);
            await _auditLogsRepository.AddLog(log);

            return Result<bool>.SuccessResult(true);
        }

        public async Task<Result<string>> ChangeUserPasswordAsync(ChangeUserPasswordCommand command)
        {
            var user = await _userRepository.GetUserByEmailAsync(command.Email);
            if (user == null)
                return Result<string>.FailureResult("User not found");

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, command.OldPassword))
                return Result<string>.FailureResult("Old password is incorrect");

            user.PasswordHash = _passwordHasher.HashPassword(command.NewPassword);
            var result = await _userRepository.UpdateUserAsync(user);

            if (!result)
                return Result<string>.FailureResult("Failed to change password");

            var log = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.UPDATE, ResourceType.User);
            await _auditLogsRepository.AddLog(log);

            return Result<string>.SuccessResult("Password changed successfully");
        }
    }
}
