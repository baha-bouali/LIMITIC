using LIMTIC.Application.Contracts.Commands.Profiles;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Profiles;

namespace LIMTIC.Application.Abstractions.Profiles
{
    public interface IPhDStudentProfileService
    {
        Task<Result<List<PhDStudentProfileDto>>> GetAllAsync();
        Task<Result<PhDStudentProfileDto>> GetByUserIdAsync(Guid userId);
        Task<Result<PhDStudentProfileCommandResponse>> UpdateAsync(UpdatePhDStudentProfileCommand command);
        Task<Result<bool>> DeleteAsync(Guid userId);
    }
}
