using LIMTIC.Application.Commands.CreateUser;
using LIMTIC.Application.Commands.GetUser;
using LIMTIC.Application.Commands.ChangeUserPassword;
using LIMTIC.Application.Contracts.UserManagement;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Application.Services.UserManagement
{
    public class UsersManagementService : IUsersManagementService
    {
        private readonly IUserRepository _userRepository;

        public UsersManagementService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<CreateUserCommandResponse>> CreateUserAsync(CreateUserCommand command)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(command.Email);
            if (existingUser != null)
                return Result<CreateUserCommandResponse>.FailureResult("Email already registered");

            var user = User.Create(
                email: command.Email,
                firstName: command.FirstName,
                lastName: command.LastName,
                passwordHash: command.PasswordHash,
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
        public async Task<Result<ChangeUserPasswordCommandResponse>> ChangeUserPasswordAsync(ChangeUserPasswordCommand command)
        {
            var user = await _userRepository.GetUserByEmailAsync(command.Email);
            if (user == null)
                return Result<ChangeUserPasswordCommandResponse>.FailureResult("User not found");
            if (user.PasswordHash != command.OldPassword)
                return Result<ChangeUserPasswordCommandResponse>.FailureResult("Old password is incorrect");
            var result = await _userRepository.UpdateUserPassword(user, command.NewPassword);
            return result ? Result<ChangeUserPasswordCommandResponse>.SuccessResult(new ChangeUserPasswordCommandResponse
            {
                User = user
            }) : Result<ChangeUserPasswordCommandResponse>.FailureResult("Failed to change password");
        }
    }
}
