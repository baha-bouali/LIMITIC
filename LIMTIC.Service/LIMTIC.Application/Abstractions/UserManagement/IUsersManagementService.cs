using LIMTIC.Application.Contracts.Commands.ChangeUserPassword;
using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Commands.GetUser;
using LIMTIC.Application.Contracts.Commands.UpdateUserRole;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.UserManagement;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Abstractions.UserManagement
{
    public interface IUsersManagementService
    {
        Task<Result<CreateUserCommandResponse>> CreateUserAsync(CreateUserCommand command);
        Task<Result<GetUserCommandResponse>> GetUserByIdAsync(Guid userId);
        Task<Result<string>> ChangeUserPasswordAsync(ChangeUserPasswordCommand command);
        Task<Result<bool>> ActivateUserAsync(Guid userId);
        Task<Result<bool>> DeactivateUserAsync(Guid userId);
        Task<Result<bool>> DeleteUserAsync(Guid userId);
        Task<Result<bool>> UpdateUserRoleAsync(UpdateUserRoleCommand command);
        Task<Result<string>> UpdateUserAvatarAsync(Guid userId, Stream fileStream, string fileName, string contentType);
        Task<Result<GetUsersResult>> GetUsersAsync(UserRole? role, bool? isActive, string? search, int page, int limit);
    }
}
