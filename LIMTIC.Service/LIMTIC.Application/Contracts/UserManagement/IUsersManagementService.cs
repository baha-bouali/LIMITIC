using LIMTIC.Application.Commands.ChangeUserPassword;
using LIMTIC.Application.Commands.CreateUser;
using LIMTIC.Application.Commands.GetUser;
using LIMTIC.Domain.Shared;

namespace LIMTIC.Application.Contracts.UserManagement
{
    public interface IUsersManagementService
    {
        public Task<Result<CreateUserCommandResponse>> CreateUserAsync(CreateUserCommand command);
        public Task<Result<GetUserCommandResponse>> GetUserByIdAsync(Guid userId);
        public Task<Result<ChangeUserPasswordCommandResponse>> ChangeUserPasswordAsync(ChangeUserPasswordCommand command);
       
    }
}
