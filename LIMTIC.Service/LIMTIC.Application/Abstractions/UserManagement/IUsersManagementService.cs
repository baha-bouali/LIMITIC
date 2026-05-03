using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Commands.GetUser;
using LIMTIC.Application.Commands.ChangeUserPassword;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Application.Abstractions.UserManagement
{
    public interface IUsersManagementService
    {
        public Task<Result<CreateUserCommandResponse>> CreateUserAsync(CreateUserCommand command);
        public Task<Result<GetUserCommandResponse>> GetUserByIdAsync(Guid userId);
        //public Task<Result<ChangeUserPasswordCommandResponse>> ChangeUserPasswordAsync(ChangeUserPasswordCommand command);
        //public Task<Result<string>> ForgetPasswordAsync(ForgetPasswordCommand command);
        //public Task<Result<VerifyResetCodeCommandResponse>> VerifyResetTokenAsync(VerifyResetCodeCommand command);
        //public Task<Result<string>> ResetPasswordAsync(ResetPasswordCommand command);

    
    }
}
