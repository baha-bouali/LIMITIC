using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using LIMTIC.Application.Commands.ChangeUserPassword;
using LIMTIC.Application.Commands.CreateUser;
using LIMTIC.Application.Commands.GetUser;
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
    }
}
//        public async Task<Result<ChangeUserPasswordCommandResponse>> ChangeUserPasswordAsync(ChangeUserPasswordCommand command)
//        {
//            var user = await _userRepository.GetUserByEmailAsync(command.Email);
//            if (user == null)
//                return Result<ChangeUserPasswordCommandResponse>.FailureResult("User not found");
//            if (user.PasswordHash != command.OldPassword)
//                return Result<ChangeUserPasswordCommandResponse>.FailureResult("Old password is incorrect");
//            var result = await _userRepository.UpdateUserPassword(user, command.NewPassword);
//            return result ? Result<ChangeUserPasswordCommandResponse>.SuccessResult(new ChangeUserPasswordCommandResponse
//            {
//                User = user
//            }) : Result<ChangeUserPasswordCommandResponse>.FailureResult("Failed to change password");
//        }
//        public async Task<Result<string>> ForgetPasswordAsync(ForgetPasswordCommand command)
//        {
//            var user = await _userRepository.GetUserByEmailAsync(command.email);
//            if (user == null)
//                return Result<string>.FailureResult("User not found");
//            var otp = GenerateOtp();
//            user.OTPToken = otp;
//            user.OTPTokenExpiry= DateTime.UtcNow.AddSeconds(120);
//            var updateResult = await _userRepository.UpdateUserAsync(user);
//            if (!updateResult)
//                return Result<string>.FailureResult("Failed to generate reset token");

//            return Result<string>.SuccessResult(otp);
//        }
//        public async Task<Result<VerifyResetCodeCommandResponse>> VerifyResetTokenAsync(VerifyResetCodeCommand command)
//        {
//            var user = await _userRepository.GetUserByEmailAsync(command.email);
//            if(user== null)
//                return Result<VerifyResetCodeCommandResponse>.FailureResult("User not found");
//            var currentTime = DateTime.UtcNow;
//            if(user.OTPToken != command.otpToken || user.OTPTokenExpiry < currentTime)
//                return Result<VerifyResetCodeCommandResponse>.FailureResult("Invalid or expired OTP token");
//            var resetToken = GenerateResetToken();
//            user.ResetPasswordToken = resetToken;
//            user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(10);
//            var updateResult = await _userRepository.UpdateUserAsync(user);
//            if (!updateResult)
//                return Result<VerifyResetCodeCommandResponse>.FailureResult("Failed to verify OTP ");
//            return Result<VerifyResetCodeCommandResponse>.SuccessResult(new VerifyResetCodeCommandResponse
//            {
//                ResetToken = resetToken
//            });

//        }
//        public Task<Result<string>> ResetPasswordAsync(ResetPasswordCommand command)
//        {
//           var user = _userRepository.GetUserByEmailAsync(command.email).Result;
//            if (user == null)
//                return Task.FromResult(Result<string>.FailureResult("User not found"));
//            var currentTime = DateTime.UtcNow;
//            if (user.ResetPasswordToken != command.ResetToken || user.ResetPasswordTokenExpiry < currentTime)
//                return Task.FromResult(Result<string>.FailureResult("Invalid or expired reset token"));
//            user.PasswordHash = command.NewPassword;
//            user.ResetPasswordToken = null;
//            user.ResetPasswordTokenExpiry = null;
//            var updateResult = _userRepository.UpdateUserAsync(user).Result;
//            if (!updateResult)
//                return Task.FromResult(Result<string>.FailureResult("Failed to reset password"));
//            return Task.FromResult(Result<string>.SuccessResult("Password reset successful"));

//        }
//        private string GenerateOtp()
//        {
//            var random = new Random();
//            return random.Next(100000, 999999).ToString();
//        }
//        private static string GenerateResetToken()
//        {
//            var bytes = new byte[32];
//            RandomNumberGenerator.Fill(bytes);
//            return Convert.ToBase64String(bytes)
//                          .Replace("+", "-")
//                          .Replace("/", "_")
//                          .TrimEnd('=');
//        }
//    }
//}
