using LIMTIC.Application.Contracts.Commands.ResearchAxis;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.ResearchAxis;

namespace LIMTIC.Application.Abstractions
{
    public interface IResearchAxisService
    {
        Task<Result<List<ResearchAxisDto>>> GetAllAsync();
        Task<Result<ResearchAxisDto>> GetByIdAsync(Guid id);
        Task<Result<ResearchAxisDto>> CreateAsync(CreateResearchAxisCommand command);
        Task<Result<ResearchAxisDto>> UpdateAsync(UpdateResearchAxisCommand command);
        Task<Result<bool>> DeleteAsync(Guid id);
        Task<Result<bool>> AddMemberAsync(Guid axisId, Guid userId);
        Task<Result<bool>> RemoveMemberAsync(Guid axisId, Guid userId);
    }
}
