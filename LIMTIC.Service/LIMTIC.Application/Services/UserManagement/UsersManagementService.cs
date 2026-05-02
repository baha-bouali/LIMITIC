using FluentValidation;
using LIMTIC.Application.Abstractions.Security;
using LIMTIC.Application.Commands.CreateUser;
using LIMTIC.Application.Commands.GetUser;
using LIMTIC.Application.Contracts.UserManagement;
using LIMTIC.Application.Helpers;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Application.Services.UserManagement
{
    public class UsersManagementService : IUsersManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IValidator<CreateUserCommand> _createUserCommandValidator;

        public UsersManagementService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IValidator<CreateUserCommand> createUserCommandValidator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _createUserCommandValidator = createUserCommandValidator;
        }

        public async Task<Result<CreateUserCommandResponse>> CreateUserAsync(CreateUserCommand command)
        {
            var validationResult = _createUserCommandValidator.Validate(command);
            if (!validationResult.IsValid)
                return Result<CreateUserCommandResponse>.ValidationFailureResult(ValidationHelper.ParseValidationErrors(validationResult));

            var existingUser = await _userRepository.GetUserByEmailAsync(command.Email);
            if (existingUser != null)
                return Result<CreateUserCommandResponse>.FailureResult("Email already registered");

            var user = User.Create(
                email: command.Email, 
                firstName: command.FirstName, 
                lastName: command.LastName, 
                passwordHash: _passwordHasher.HashPassword(command.Password), 
                isActive: true,
                role: command.Role);

            var result = await _userRepository.AddUserAsync(user.Data);
            return result ? Result<CreateUserCommandResponse>.SuccessResult(new CreateUserCommandResponse
            {
                User = user.Data,
            }) : Result<CreateUserCommandResponse>.FailureResult("Failed to create user");
        }

        public async Task<Result<GetUserCommandResponse>> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return user != null ? Result<GetUserCommandResponse>.SuccessResult(new GetUserCommandResponse
            {
                User = user
            }) : Result<GetUserCommandResponse>.FailureResult("User not found");
        }
    }
}
