using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Commands.GetUser;
using LIMTIC.Domain.Shared;
using LIMTIC.Application.Contracts.Commands.ChangeUserPassword;
namespace LIMTIC.Application.Abstractions.UserManagement
{
    public interface IUsersManagementService
    {
        public Task<Result<CreateUserCommandResponse>> CreateUserAsync(CreateUserCommand command);
        public Task<Result<GetUserCommandResponse>> GetUserByIdAsync(Guid userId);
        public Task<Result<string>> ChangeUserPasswordAsync(ChangeUserPasswordCommand command);
     
    
    }
}
