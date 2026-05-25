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
    public class PhDStudentProfileService : IPhDStudentProfileService
    {
        private readonly IPhDStudentRepository _phDStudentRepository;
        private readonly IResearcherRepository _researcherRepository;
        private readonly IResearchAxisRepository _researchAxisRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProfileMapper _profileMapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<UpdatePhDStudentProfileCommand> _updateValidator;
        private readonly IAuditLogsRepository _auditLogsRepository;

        public PhDStudentProfileService(
            IPhDStudentRepository phDStudentRepository,
            IResearcherRepository researcherRepository,
            IResearchAxisRepository researchAxisRepository,
            IUserRepository userRepository,
            IProfileMapper profileMapper,
            ICurrentUserService currentUserService,
            IValidator<UpdatePhDStudentProfileCommand> updateValidator,
            IAuditLogsRepository auditLogsRepository)
        {
            _phDStudentRepository = phDStudentRepository;
            _researcherRepository = researcherRepository;
            _researchAxisRepository = researchAxisRepository;
            _userRepository = userRepository;
            _profileMapper = profileMapper;
            _currentUserService = currentUserService;
            _updateValidator = updateValidator;
            _auditLogsRepository = auditLogsRepository;
        }

        public async Task<Result<List<PhDStudentProfileDto>>> GetAllAsync()
        {
            var students = await _phDStudentRepository.GetAllAsync();
            var dtos = students.Select(_profileMapper.MapToPhDStudentProfileDto).ToList();
            return Result<List<PhDStudentProfileDto>>.SuccessResult(dtos);
        }

        public async Task<Result<PhDStudentProfileDto>> GetByUserIdAsync(Guid userId)
        {
            var phDStudent = await _phDStudentRepository.GetByUserIdAsync(userId);
            if (phDStudent is null)
                return Result<PhDStudentProfileDto>.FailureResult("PhD student profile not found");

            return Result<PhDStudentProfileDto>.SuccessResult(_profileMapper.MapToPhDStudentProfileDto(phDStudent));
        }

        public async Task<Result<PhDStudentProfileCommandResponse>> UpdateAsync(UpdatePhDStudentProfileCommand command)
        {
            var validationResult = _updateValidator.Validate(command);
            if (!validationResult.IsValid)
                return Result<PhDStudentProfileCommandResponse>.ValidationFailureResult(
                    ValidationHelper.ParseValidationErrors(validationResult));

            // Authorization: only admin or the PhD student themselves
            var currentUserId = _currentUserService.UserId;
            var currentRole = _currentUserService.Role;
            var isAdmin = Enum.TryParse<UserRole>(currentRole, out var role) &&
                (role is UserRole.SuperAdmin or UserRole.Admin);

            if (!isAdmin && currentUserId != command.UserId)
                return Result<PhDStudentProfileCommandResponse>.FailureResult("You are not authorized to update this profile");

            var phDStudent = await _phDStudentRepository.GetByUserIdAsync(command.UserId);
            if (phDStudent is null)
                return Result<PhDStudentProfileCommandResponse>.FailureResult("PhD student profile not found");

            // Validate supervisor if provided
            if (command.SupervisorId.HasValue)
            {
                var supervisorExists = await _researcherRepository.ExistsAsync(command.SupervisorId.Value);
                if (!supervisorExists)
                    return Result<PhDStudentProfileCommandResponse>.FailureResult("Supervisor not found");
            }

            // Update scalar fields
            phDStudent.ThesisSubject = command.ThesisSubject?.Trim();
            phDStudent.EnrollmentYear = command.EnrollmentYear;
            phDStudent.SupervisorId = command.SupervisorId;

            // Update research axes if provided
            if (command.ResearchAxisIds is not null)
            {
                phDStudent.ResearchAxes.Clear();
                if (command.ResearchAxisIds.Count > 0)
                {
                    var axes = await _researchAxisRepository.GetByIdsAsync(command.ResearchAxisIds);
                    foreach (var axis in axes)
                        phDStudent.ResearchAxes.Add(axis);
                }
            }

            var saved = await _phDStudentRepository.UpdateAsync(phDStudent);
            if (!saved)
                return Result<PhDStudentProfileCommandResponse>.FailureResult("Failed to update PhD student profile");

            // Reload to get updated supervisor navigation
            var updated = await _phDStudentRepository.GetByUserIdAsync(command.UserId);

            var profileLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.UPDATE, ResourceType.User);
            await _auditLogsRepository.AddLog(profileLog);

            return Result<PhDStudentProfileCommandResponse>.SuccessResult(new PhDStudentProfileCommandResponse
            {
                Profile = _profileMapper.MapToPhDStudentProfileDto(updated!)
            });
        }

        public async Task<Result<bool>> DeleteAsync(Guid userId)
        {
            var phDStudent = await _phDStudentRepository.GetByUserIdAsync(userId);
            if (phDStudent is null)
                return Result<bool>.FailureResult("PhD student profile not found");

            var deleted = await _phDStudentRepository.DeleteAsync(phDStudent);
            if (!deleted)
                return Result<bool>.FailureResult("Failed to delete PhD student profile");

            // Reset user role back to Visitor
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user is not null)
            {
                user.Role = UserRole.Visitor;
                var updated = await _userRepository.UpdateUserAsync(user);
                if (!updated)
                    return Result<bool>.FailureResult("Failed to reset user role");
            }

            var profileLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.UPDATE, ResourceType.User);
            await _auditLogsRepository.AddLog(profileLog);

            return Result<bool>.SuccessResult(true);
        }
    }
}
