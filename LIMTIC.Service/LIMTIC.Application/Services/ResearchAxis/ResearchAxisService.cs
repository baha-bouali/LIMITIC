using FluentValidation;
using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Contracts.Commands.ResearchAxis;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Profiles;
using LIMTIC.Application.Helpers;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.ResearchAxis;

namespace LIMTIC.Application.Services.ResearchAxis
{
    public class ResearchAxisService : IResearchAxisService
    {
        private readonly IResearchAxisRepository _researchAxisRepository;
        private readonly IValidator<CreateResearchAxisCommand> _createValidator;
        private readonly IValidator<UpdateResearchAxisCommand> _updateValidator;

        public ResearchAxisService(
            IResearchAxisRepository researchAxisRepository,
            IValidator<CreateResearchAxisCommand> createValidator,
            IValidator<UpdateResearchAxisCommand> updateValidator)
        {
            _researchAxisRepository = researchAxisRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<Result<List<ResearchAxisDto>>> GetAllAsync()
        {
            var axes = await _researchAxisRepository.GetAllAsync();
            return Result<List<ResearchAxisDto>>.SuccessResult(axes.Select(MapToDto).ToList());
        }

        public async Task<Result<ResearchAxisDto>> GetByIdAsync(Guid id)
        {
            var axis = await _researchAxisRepository.GetByIdAsync(id);
            if (axis is null)
                return Result<ResearchAxisDto>.FailureResult("Research axis not found");

            return Result<ResearchAxisDto>.SuccessResult(MapToDto(axis));
        }

        public async Task<Result<ResearchAxisDto>> CreateAsync(CreateResearchAxisCommand command)
        {
            var validation = _createValidator.Validate(command);
            if (!validation.IsValid)
                return Result<ResearchAxisDto>.ValidationFailureResult(ValidationHelper.ParseValidationErrors(validation));

            var entity = new ResearchAxisEntity
            {
                Id = Guid.NewGuid(),
                Title = command.Title.Trim(),
                Description = command.Description.Trim(),
                Themes = command.Themes
            };

            var added = await _researchAxisRepository.AddAsync(entity);
            return added
                ? Result<ResearchAxisDto>.SuccessResult(MapToDto(entity))
                : Result<ResearchAxisDto>.FailureResult("Failed to create research axis");
        }

        public async Task<Result<ResearchAxisDto>> UpdateAsync(UpdateResearchAxisCommand command)
        {
            var validation = _updateValidator.Validate(command);
            if (!validation.IsValid)
                return Result<ResearchAxisDto>.ValidationFailureResult(ValidationHelper.ParseValidationErrors(validation));

            var entity = await _researchAxisRepository.GetByIdAsync(command.Id);
            if (entity is null)
                return Result<ResearchAxisDto>.FailureResult("Research axis not found");

            entity.Title = command.Title.Trim();
            entity.Description = command.Description.Trim();
            entity.Themes = command.Themes;

            var updated = await _researchAxisRepository.UpdateAsync(entity);
            return updated
                ? Result<ResearchAxisDto>.SuccessResult(MapToDto(entity))
                : Result<ResearchAxisDto>.FailureResult("Failed to update research axis");
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var entity = await _researchAxisRepository.GetByIdAsync(id);
            if (entity is null)
                return Result<bool>.FailureResult("Research axis not found");

            var deleted = await _researchAxisRepository.DeleteAsync(entity);
            return deleted
                ? Result<bool>.SuccessResult(true)
                : Result<bool>.FailureResult("Failed to delete research axis");
        }

        private static ResearchAxisDto MapToDto(ResearchAxisEntity e) => new()
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Themes = e.Themes
        };
    }
}
