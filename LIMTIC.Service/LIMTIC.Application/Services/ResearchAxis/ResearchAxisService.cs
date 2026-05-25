using FluentValidation;
using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Contracts.Commands.ResearchAxis;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Profiles;
using LIMTIC.Application.Helpers;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.ResearchAxis;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Services.ResearchAxis
{
    public class ResearchAxisService : IResearchAxisService
    {
        private readonly IResearchAxisRepository _researchAxisRepository;
        private readonly IResearcherRepository _researcherRepository;
        private readonly IValidator<CreateResearchAxisCommand> _createValidator;
        private readonly IValidator<UpdateResearchAxisCommand> _updateValidator;
        private readonly IAuditLogsRepository _auditLogsRepository;
        private readonly ICurrentUserService _currentUserService;

        public ResearchAxisService(
            IResearchAxisRepository researchAxisRepository,
            IResearcherRepository researcherRepository,
            IValidator<CreateResearchAxisCommand> createValidator,
            IValidator<UpdateResearchAxisCommand> updateValidator,
            IAuditLogsRepository auditLogsRepository,
            ICurrentUserService currentUserService)
        {
            _researchAxisRepository = researchAxisRepository;
            _researcherRepository = researcherRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _auditLogsRepository = auditLogsRepository;
            _currentUserService = currentUserService;
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

            if (command.ResponsibleId.HasValue && !await _researcherRepository.ExistsAsync(command.ResponsibleId.Value))
                return Result<ResearchAxisDto>.FailureResult("Responsible user is not a researcher");

            var entity = new ResearchAxisEntity
            {
                Id = Guid.NewGuid(),
                Title = command.Title.Trim(),
                Description = command.Description.Trim(),
                Themes = command.Themes,
                Color = command.Color?.Trim(),
                ResponsibleId = command.ResponsibleId
            };

            if (command.MemberIds != null && command.MemberIds.Count > 0)
            {
                var members = await LoadResearchersAsync(command.MemberIds);
                if (members is null)
                    return Result<ResearchAxisDto>.FailureResult("One or more member IDs do not belong to a researcher");
                entity.Researchers = members;
            }

            var added = await _researchAxisRepository.AddAsync(entity);
            if (!added)
                return Result<ResearchAxisDto>.FailureResult("Failed to create research axis");

            var axisLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.CREATE, ResourceType.Axe);
            await _auditLogsRepository.AddLog(axisLog);

            var created = await _researchAxisRepository.GetByIdAsync(entity.Id);
            return Result<ResearchAxisDto>.SuccessResult(MapToDto(created!));
        }

        public async Task<Result<ResearchAxisDto>> UpdateAsync(UpdateResearchAxisCommand command)
        {
            var validation = _updateValidator.Validate(command);
            if (!validation.IsValid)
                return Result<ResearchAxisDto>.ValidationFailureResult(ValidationHelper.ParseValidationErrors(validation));

            var entity = await _researchAxisRepository.GetByIdAsync(command.Id);
            if (entity is null)
                return Result<ResearchAxisDto>.FailureResult("Research axis not found");

            if (command.ResponsibleId.HasValue && !await _researcherRepository.ExistsAsync(command.ResponsibleId.Value))
                return Result<ResearchAxisDto>.FailureResult("Responsible user is not a researcher");

            entity.Title = command.Title.Trim();
            entity.Description = command.Description.Trim();
            entity.Themes = command.Themes;
            entity.Color = command.Color?.Trim();
            entity.ResponsibleId = command.ResponsibleId;

            if (command.MemberIds != null)
            {
                var members = await LoadResearchersAsync(command.MemberIds);
                if (members is null)
                    return Result<ResearchAxisDto>.FailureResult("One or more member IDs do not belong to a researcher");
                entity.Researchers = members;
            }

            var updated = await _researchAxisRepository.UpdateAsync(entity);
            if (!updated)
                return Result<ResearchAxisDto>.FailureResult("Failed to update research axis");

            var axisLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.UPDATE, ResourceType.Axe);
            await _auditLogsRepository.AddLog(axisLog);

            return Result<ResearchAxisDto>.SuccessResult(MapToDto(entity));
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var entity = await _researchAxisRepository.GetByIdAsync(id);
            if (entity is null)
                return Result<bool>.FailureResult("Research axis not found");

            if (entity.Publications.Count > 0)
                return Result<bool>.FailureResult("Cannot delete axis with associated publications");

            var deleted = await _researchAxisRepository.DeleteAsync(entity);
            if (!deleted)
                return Result<bool>.FailureResult("Failed to delete research axis");

            var axisLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.DELETE, ResourceType.Axe);
            await _auditLogsRepository.AddLog(axisLog);

            return Result<bool>.SuccessResult(true);
        }

        public async Task<Result<bool>> AddMemberAsync(Guid axisId, Guid userId)
        {
            if (!await _researchAxisRepository.ExistsAsync(axisId))
                return Result<bool>.FailureResult("Research axis not found");

            if (!await _researcherRepository.ExistsAsync(userId))
                return Result<bool>.FailureResult("User is not a researcher");

            var added = await _researchAxisRepository.AddMemberAsync(axisId, userId);
            if (!added)
                return Result<bool>.FailureResult("Failed to add member");

            var memberLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.UPDATE, ResourceType.Axe);
            await _auditLogsRepository.AddLog(memberLog);

            return Result<bool>.SuccessResult(true);
        }

        public async Task<Result<bool>> RemoveMemberAsync(Guid axisId, Guid userId)
        {
            if (!await _researchAxisRepository.ExistsAsync(axisId))
                return Result<bool>.FailureResult("Research axis not found");

            var removed = await _researchAxisRepository.RemoveMemberAsync(axisId, userId);
            if (!removed)
                return Result<bool>.FailureResult("Member not found in this axis");

            var memberLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.UPDATE, ResourceType.Axe);
            await _auditLogsRepository.AddLog(memberLog);

            return Result<bool>.SuccessResult(true);
        }

        private async Task<ICollection<ResearcherEntity>?> LoadResearchersAsync(List<Guid> ids)
        {
            var researchers = new List<ResearcherEntity>();
            foreach (var id in ids)
            {
                var r = await _researcherRepository.GetByUserIdAsync(id);
                if (r is null) return null;
                researchers.Add(r);
            }
            return researchers;
        }

        private static ResearchAxisDto MapToDto(ResearchAxisEntity e) => new()
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Themes = e.Themes,
            Color = e.Color,
            ResponsibleId = e.ResponsibleId,
            ResponsibleName = e.Responsible?.User is { } u
                ? $"{u.FirstName} {u.LastName}".Trim()
                : null,
            PublicationsCount = e.Publications.Count,
            Members = e.Researchers.Select(r => new AxisMemberDto
            {
                Id = r.Id,
                FirstName = r.User?.FirstName ?? string.Empty,
                LastName = r.User?.LastName ?? string.Empty
            }).ToList()
        };
    }
}
