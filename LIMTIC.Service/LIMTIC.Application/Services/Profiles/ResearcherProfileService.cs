using FluentValidation;
using LIMTIC.Application.Abstractions;
using LIMTIC.Application.Abstractions.Profiles;
using LIMTIC.Application.Contracts.Commands.Profiles;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.Profiles;
using LIMTIC.Application.DTOs.ResearchAxis;
using LIMTIC.Application.Helpers;

using LIMTIC.Application.Mappers.ProfileMapper;
using LIMTIC.Domain.Abstractions.AuditLogs;
using LIMTIC.Domain.Abstractions.ResearchAxis;
using LIMTIC.Domain.Abstractions.Users;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Services.Profiles
{
    public class ResearcherProfileService : IResearcherProfileService
    {
        private readonly IResearcherRepository _researcherRepository;
        private readonly IResearchAxisRepository _researchAxisRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProfileMapper _profileMapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<UpdateResearcherProfileCommand> _updateValidator;
        private readonly IAuditLogsRepository _auditLogsRepository;

        public ResearcherProfileService(
            IResearcherRepository researcherRepository,
            IResearchAxisRepository researchAxisRepository,
            IUserRepository userRepository,
            IProfileMapper profileMapper,
            ICurrentUserService currentUserService,
            IValidator<UpdateResearcherProfileCommand> updateValidator,
            IAuditLogsRepository auditLogsRepository)
        {
            _researcherRepository = researcherRepository;
            _researchAxisRepository = researchAxisRepository;
            _userRepository = userRepository;
            _profileMapper = profileMapper;
            _currentUserService = currentUserService;
            _updateValidator = updateValidator;
            _auditLogsRepository = auditLogsRepository;
        }

        public async Task<Result<List<ResearcherProfileDto>>> GetAllAsync()
        {
            var researchers = await _researcherRepository.GetAllAsync();
            var dtos = researchers.Select(_profileMapper.MapToResearcherProfileDto).ToList();
            return Result<List<ResearcherProfileDto>>.SuccessResult(dtos);
        }

        public async Task<Result<ResearcherProfileDto>> GetByUserIdAsync(Guid userId)
        {
            ResearcherEntity? researcher = null;
            try
            {
                researcher = await _researcherRepository.GetByUserIdAsync(userId);
            }
            catch (Exception)
            {
                // If repository fails due to missing schema (join table), fall back to returning
                // a minimal profile using the User entity to avoid returning 500.
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user is null)
                    return Result<ResearcherProfileDto>.FailureResult("Researcher profile not found");

                var minimal = new ResearcherProfileDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    AvatarBlobName = user.AvatarBlobName,
                    // Other researcher fields unavailable in fallback
                    Rank = null,
                    Specialty = null,
                    Office = null,
                    PhoneNumber = null,
                    Biography = null,
                    Orcid = null,
                    GoogleScholar = null,
                    ResearchGate = null,
                    LinkedIn = null,
                    ResearchAxes = new List<ResearchAxisDto>()
                };

                return Result<ResearcherProfileDto>.SuccessResult(minimal);
            }

            if (researcher is null)
                return Result<ResearcherProfileDto>.FailureResult("Researcher profile not found");

            return Result<ResearcherProfileDto>.SuccessResult(_profileMapper.MapToResearcherProfileDto(researcher));
        }

        public async Task<Result<ResearcherProfileCommandResponse>> UpdateAsync(UpdateResearcherProfileCommand command)
        {
            var validationResult = _updateValidator.Validate(command);
            if (!validationResult.IsValid)
                return Result<ResearcherProfileCommandResponse>.ValidationFailureResult(
                    ValidationHelper.ParseValidationErrors(validationResult));

            // Authorization: only admin or the researcher themselves
            var currentUserId = _currentUserService.UserId;
            var currentRole = _currentUserService.Role;
            var isAdmin = Enum.TryParse<UserRole>(currentRole, out var role) &&
                (role is UserRole.SuperAdmin or UserRole.Admin);

            if (!isAdmin && currentUserId != command.UserId)
                return Result<ResearcherProfileCommandResponse>.FailureResult("You are not authorized to update this profile");

            var researcher = await _researcherRepository.GetByUserIdAsync(command.UserId);
            if (researcher is null)
                return Result<ResearcherProfileCommandResponse>.FailureResult("Researcher profile not found");

            // Update scalar fields
            researcher.Rank = command.Rank.Trim();
            researcher.Specialty = command.Specialty.Trim();
            researcher.Office = command.Office.Trim();
            researcher.PhoneNumber = command.PhoneNumber.Trim();
            researcher.Biography = command.Biography?.Trim();
            researcher.Orcid = command.Orcid?.Trim();
            researcher.GoogleScholar = command.GoogleScholar?.Trim();
            researcher.ResearchGate = command.ResearchGate?.Trim();
            researcher.LinkedIn = command.LinkedIn?.Trim();

            // Update research axes if provided
            if (command.ResearchAxisIds is not null)
            {
                researcher.ResearchAxes.Clear();
                if (command.ResearchAxisIds.Count > 0)
                {
                    var axes = await _researchAxisRepository.GetByIdsAsync(command.ResearchAxisIds);
                    foreach (var axis in axes)
                        researcher.ResearchAxes.Add(axis);
                }
            }

            var saved = await _researcherRepository.UpdateAsync(researcher);
            if (!saved)
                return Result<ResearcherProfileCommandResponse>.FailureResult("Failed to update researcher profile");

            var profileLog = AuditLogHelper.CreateAuditLog(_currentUserService.UserId, ActionType.UPDATE, ResourceType.User);
            await _auditLogsRepository.AddLog(profileLog);

            return Result<ResearcherProfileCommandResponse>.SuccessResult(new ResearcherProfileCommandResponse
            {
                Profile = _profileMapper.MapToResearcherProfileDto(researcher)
            });
        }

        public async Task<Result<bool>> DeleteAsync(Guid userId)
        {
            var researcher = await _researcherRepository.GetByUserIdAsync(userId);
            if (researcher is null)
                return Result<bool>.FailureResult("Researcher profile not found");

            var deleted = await _researcherRepository.DeleteAsync(researcher);
            if (!deleted)
                return Result<bool>.FailureResult("Failed to delete researcher profile");

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
