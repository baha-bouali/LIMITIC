using LIMTIC.Application.Contracts.Commands.Profiles;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Profiles;

namespace LIMTIC.Application.Abstractions.Profiles
{
    public interface IMasterianProfileService
    {
        Task<Result<List<MasterianProfileDto>>> GetAllAsync();
        Task<Result<MasterianProfileDto>> GetByUserIdAsync(Guid userId);
        Task<Result<MasterianProfileCommandResponse>> UpdateAsync(UpdateMasterianProfileCommand command);
        Task<Result<bool>> DeleteAsync(Guid userId);
    }
}
