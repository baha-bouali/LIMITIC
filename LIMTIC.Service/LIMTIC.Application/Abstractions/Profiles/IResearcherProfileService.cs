using LIMTIC.Application.Contracts.Commands.Profiles;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Profiles;

namespace LIMTIC.Application.Abstractions.Profiles
{
    public interface IResearcherProfileService
    {
        Task<Result<List<ResearcherProfileDto>>> GetAllAsync();
        Task<Result<ResearcherProfileDto>> GetByUserIdAsync(Guid userId);
        Task<Result<ResearcherProfileDto>> UpdateAsync(UpdateResearcherProfileCommand command);
        Task<Result<bool>> DeleteAsync(Guid userId);
    }
}
