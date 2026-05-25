using FluentValidation;
using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Profiles;
using LIMTIC.Application.Contracts.Commands.Profiles;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Profiles;
using LIMTIC.Application.Helpers;
using LIMTIC.Application.Mappers.ProfileMapper;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Services.Profiles
{
    public class MasterianProfileService : IMasterianProfileService
    {
        private readonly IMasterianRepository _masterianRepository;
        private readonly IResearcherRepository _researcherRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProfileMapper _profileMapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<UpdateMasterianProfileCommand> _updateValidator;
        private readonly IAuditLogsRepository _auditLogsRepository;

        public MasterianProfileService(
            IMasterianRepository masterianRepository,
            IResearcherRepository researcherRepository,
            IUserRepository userRepository,
            IProfileMapper profileMapper,
            ICurrentUserService currentUserService,
            IValidator<UpdateMasterianProfileCommand> updateValidator,
            IAuditLogsRepository auditLogsRepository)
        {
            _masterianRepository = masterianRepository;
            _researcherRepository = researcherRepository;
            _userRepository = userRepository;
            _profileMapper = profileMapper;
            _currentUserService = currentUserService;
            _updateValidator = updateValidator;
            _auditLogsRepository = auditLogsRepository;
        }

        public async Task<Result<MasterianProfileDto>> GetByUserIdAsync(Guid userId)
        {
            var masterian = await _masterianRepository.GetByUserIdAsync(userId);
            if (masterian is null)
                return Result<MasterianProfileDto>.FailureResult("Masterian profile not found");

            return Result<MasterianProfileDto>.SuccessResult(_profileMapper.MapToMasterianProfileDto(masterian));
        }

        public async Task<Result<MasterianProfileCommandResponse>> UpdateAsync(UpdateMasterianProfileCommand command)
        {
            var validationResult = _updateValidator.Validate(command);
            if (!validationResult.IsValid)
                return Result<MasterianProfileCommandResponse>.ValidationFailureResult(
                    ValidationHelper.ParseValidationErrors(validationResult));

            // Authorization: only admin or the masterian themselves
            var currentUserId = _currentUserService.UserId;
            var currentRole = _currentUserService.Role;
            var isAdmin = Enum.TryParse<UserRole>(currentRole, out var role) &&
                (role is UserRole.SuperAdmin or UserRole.Admin);

            if (!isAdmin && currentUserId != command.UserId)
                return Result<MasterianProfileCommandResponse>.FailureResult("You are not authorized to update this profile");

            var masterian = await _masterianRepository.GetByUserIdAsync(command.UserId);
            if (masterian is null)
                return Result<MasterianProfileCommandResponse>.FailureResult("Masterian profile not found");

            // Validate supervisor if provided
            if (command.SupervisorId.HasValue)
            {
                var supervisorExists = await _researcherRepository.ExistsAsync(command.SupervisorId.Value);
                if (!supervisorExists)
                    return Result<MasterianProfileCommandResponse>.FailureResult("Supervisor not found");
            }

            // Update scalar fields
            masterian.DissertationSubject = command.DissertationSubject.Trim();
            masterian.Cohort = command.Cohort.Trim();
            masterian.SupervisorId = command.SupervisorId;

            var saved = await _masterianRepository.UpdateAsync(masterian);
            if (!saved)
                return Result<MasterianProfileCommandResponse>.FailureResult("Failed to update masterian profile");

            // Reload to get updated supervisor navigation
            var updated = await _masterianRepository.GetByUserIdAsync(command.UserId);

            var profileLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.UPDATE, ResourceType.User);
            await _auditLogsRepository.AddLog(profileLog);

            return Result<MasterianProfileCommandResponse>.SuccessResult(new MasterianProfileCommandResponse
            {
                Profile = _profileMapper.MapToMasterianProfileDto(updated!)
            });
        }

        public async Task<Result<bool>> DeleteAsync(Guid userId)
        {
            var masterian = await _masterianRepository.GetByUserIdAsync(userId);
            if (masterian is null)
                return Result<bool>.FailureResult("Masterian profile not found");

            var deleted = await _masterianRepository.DeleteAsync(masterian);
            if (!deleted)
                return Result<bool>.FailureResult("Failed to delete masterian profile");

            // Reset user role back to Visitor
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user is not null)
            {
                user.Role = UserRole.Visitor;
                var updated = await _userRepository.UpdateUserAsync(user);
                if (!updated)
                    return Result<bool>.FailureResult("Failed to reset user role");
            }

            var profileLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.DELETE, ResourceType.User);
            await _auditLogsRepository.AddLog(profileLog);

            return Result<bool>.SuccessResult(true);
        }
    }
}
