using LIMTIC.Application.Contracts.Commands.ChangeUserPassword;
using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Commands.GetUser;
using LIMTIC.Application.DTOs;

namespace LIMTIC.Application.Abstractions.UserManagement
{
    public interface IUsersManagementService
    {
        public Task<Result<CreateUserCommandResponse>> CreateUserAsync(CreateUserCommand command);
        public Task<Result<GetUserCommandResponse>> GetUserByIdAsync(Guid userId);
        public Task<Result<string>> ChangeUserPasswordAsync(ChangeUserPasswordCommand command);
     
       
        public Task<Result<bool>> ActivateUserAsync(Guid userId);
        public Task<Result<bool>> DeactivateUserAsync(Guid userId);
    }
}
