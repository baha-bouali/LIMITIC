using FluentValidation;
using LIMTIC.Application.Abstractions.Security;
using LIMTIC.Application.Abstractions.UserManagement;
using LIMTIC.Application.Contracts.Commands.ChangeUserPassword;
using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Commands.GetUser;
using LIMTIC.Application.Contracts.Commands.UpdateUserRole;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.DTOs.UserManagement;
using LIMTIC.Application.Helpers;
using LIMTIC.Application.Mappers.UserMapper;
using LIMTIC.Application.Validations;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Services.UserManagement
{
    public class UsersManagementService : IUsersManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserMapper _userMapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IValidator<CreateUserCommand> _createUserCommandValidator;
        private readonly IResearcherRepository _researcherRepository;
        private readonly IPhDStudentRepository _phdStudentRepository;
        private readonly IMasterianRepository _masterianRepository;
        private readonly IResearchAxisRepository _researchAxisRepository;

        public UsersManagementService(
            IUserRepository userRepository,
            IUserMapper userMapper,
            IPasswordHasher passwordHasher,
            IValidator<CreateUserCommand> createUserCommandValidator,
            IResearcherRepository researcherRepository,
            IPhDStudentRepository phdStudentRepository,
            IMasterianRepository masterianRepository,
            IResearchAxisRepository researchAxisRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _createUserCommandValidator = createUserCommandValidator;
            _userMapper = userMapper;
            _researcherRepository = researcherRepository;
            _phdStudentRepository = phdStudentRepository;
            _masterianRepository = masterianRepository;
            _researchAxisRepository = researchAxisRepository;
        }

        public async Task<Result<CreateUserCommandResponse>> CreateUserAsync(CreateUserCommand command)
        {
            var validationResult = _createUserCommandValidator.Validate(command);
            if (!validationResult.IsValid)
                return Result<CreateUserCommandResponse>.ValidationFailureResult(ValidationHelper.ParseValidationErrors(validationResult));

            var existingUser = await _userRepository.GetUserByEmailAsync(command.Email);
            if (existingUser != null)
                return Result<CreateUserCommandResponse>.FailureResult("Email already registered");

            var user = UserEntity.Create(
                email: command.Email,
                firstName: command.FirstName,
                lastName: command.LastName,
                passwordHash: _passwordHasher.HashPassword(command.Password),
                isActive: command.IsActive,
                role: UserRole.Visitor);

            var result = await _userRepository.AddUserAsync(user);
            return result ? Result<CreateUserCommandResponse>.SuccessResult(new CreateUserCommandResponse
            {
                User = _userMapper.MapToUserDto(user),
            }) : Result<CreateUserCommandResponse>.FailureResult("Failed to create user");
        }

        public async Task<Result<GetUserCommandResponse>> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return user != null ? Result<GetUserCommandResponse>.SuccessResult(new GetUserCommandResponse
            {
                User = _userMapper.MapToUserDto(user)
            }) : Result<GetUserCommandResponse>.FailureResult("User not found");
        }

        public async Task<Result<bool>> ActivateUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return Result<bool>.FailureResult("User not found");

            if (user.IsActive)
                return Result<bool>.SuccessResult(data: true, message: "User is already active");

            user.IsActive = true;
            var result = await _userRepository.UpdateUserAsync(user);
            return result ? Result<bool>.SuccessResult(true) : Result<bool>.FailureResult("Failed to activate user");
        }

        public async Task<Result<bool>> DeactivateUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return Result<bool>.FailureResult("User not found");

            if (!user.IsActive)
                return Result<bool>.FailureResult("User is already deactived");

            user.IsActive = false;
            var result = await _userRepository.UpdateUserAsync(user);
            return result ? Result<bool>.SuccessResult(true) : Result<bool>.FailureResult("Failed to deactivate user");
        }

        public async Task<Result<string>> ChangeUserPasswordAsync(ChangeUserPasswordCommand command)
        {
            var user = await _userRepository.GetUserByEmailAsync(command.Email);
            if (user == null)
                return Result<string>.FailureResult("User not found");

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, command.OldPassword))
                return Result<string>.FailureResult("Old password is incorrect");

            user.PasswordHash = _passwordHasher.HashPassword(command.NewPassword);
            var result = await _userRepository.UpdateUserAsync(user);
            return result ? Result<string>.SuccessResult("Password changed successfully") : Result<string>.FailureResult("Failed to change password");
        }

        public async Task<Result<bool>> DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return Result<bool>.FailureResult("User not found");

            // Remove the role-specific profile row first to avoid FK violations
            await DeleteExistingProfileAsync(user.Id, user.Role);

            var deleted = await _userRepository.DeleteUserAsync(userId);
            return deleted
                ? Result<bool>.SuccessResult(true)
                : Result<bool>.FailureResult("Failed to delete user");
        }

        public async Task<Result<bool>> UpdateUserRoleAsync(UpdateUserRoleCommand command)
        {
            var validator = new UpdateUserRoleCommandValidator();
            var validationResult = validator.Validate(command);
            if (!validationResult.IsValid)
                return Result<bool>.ValidationFailureResult(ValidationHelper.ParseValidationErrors(validationResult));

            var user = await _userRepository.GetUserByIdAsync(command.UserId);
            if (user == null)
                return Result<bool>.FailureResult("User not found");

            // Early return if role hasn't changed
            if (user.Role == command.Role)
                return Result<bool>.SuccessResult(true);

            // Delete existing profile entry before assigning the new role
            await DeleteExistingProfileAsync(user.Id, user.Role);

            return command.Role switch
            {
                UserRole.SuperAdmin or UserRole.Admin or UserRole.Visitor =>
                    await AssignSimpleRoleAsync(user, command.Role),

                UserRole.Researcher =>
                    await AssignResearcherRoleAsync(user, command),

                UserRole.PhDStudent =>
                    await AssignPhDStudentRoleAsync(user, command),

                UserRole.Masterian =>
                    await AssignMasterianRoleAsync(user, command),

                _ => Result<bool>.FailureResult("Unsupported role")
            };
        }

        public async Task<Result<string>> UpdateUserAvatarAsync(Guid userId, Stream fileStream, string fileName, string contentType)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return Result<string>.FailureResult("User not found");

            // Generate a unique blob name: userId/filename
            var blobName = $"avatars/{userId}/{Guid.NewGuid()}_{fileName}";
            user.AvatarBlobName = blobName;

            var updated = await _userRepository.UpdateUserAsync(user);
            return updated
                ? Result<string>.SuccessResult(blobName)
                : Result<string>.FailureResult("Failed to update avatar");
        }

        public async Task<Result<GetUsersResult>> GetUsersAsync(UserRole? role, bool? isActive, string? search, int page, int limit)
        {
            var (items, total, counts) = await _userRepository.GetUsersAsync(role, isActive, search, page, limit);

            return Result<GetUsersResult>.SuccessResult(new GetUsersResult
            {
                Items = items.Select(_userMapper.MapToUserDto).ToList(),
                Total = total,
                Counts = counts,
                Page = page,
                Limit = limit
            });
        }

        // ── Private helpers ──────────────────────────────────────────────────────

        private async Task<Result<bool>> AssignSimpleRoleAsync(UserEntity user, UserRole role)
        {
            user.Role = role;
            var updated = await _userRepository.UpdateUserAsync(user);
            return updated ? Result<bool>.SuccessResult(true) : Result<bool>.FailureResult("Failed to update user role");
        }

        private async Task<Result<bool>> AssignResearcherRoleAsync(UserEntity user, UpdateUserRoleCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Rank) || string.IsNullOrWhiteSpace(command.Specialty)
                || string.IsNullOrWhiteSpace(command.Office) || string.IsNullOrWhiteSpace(command.PhoneNumber))
                return Result<bool>.FailureResult("Missing required researcher fields");

            var researcher = new ResearcherEntity
            {
                Id = user.Id,
                Rank = command.Rank!.Trim(),
                Specialty = command.Specialty!.Trim(),
                Office = command.Office!.Trim(),
                PhoneNumber = command.PhoneNumber!.Trim(),
                User = user
            };

            if (command.ResearchAxisIds != null && command.ResearchAxisIds.Any())
            {
                var axes = await _researchAxisRepository.GetByIdsAsync(command.ResearchAxisIds);
                researcher.ResearchAxes = axes;
            }

            var added = await _researcherRepository.AddAsync(researcher);
            if (!added)
                return Result<bool>.FailureResult("Failed to create researcher profile");

            user.Role = UserRole.Researcher;
            var updated = await _userRepository.UpdateUserAsync(user);
            return updated ? Result<bool>.SuccessResult(true) : Result<bool>.FailureResult("Failed to update user role");
        }

        private async Task<Result<bool>> AssignPhDStudentRoleAsync(UserEntity user, UpdateUserRoleCommand command)
        {
            if (!command.EnrollmentYear.HasValue || command.EnrollmentYear.Value <= 0)
                return Result<bool>.FailureResult("EnrollmentYear is required for PhDStudent role");

            var phd = new PhDStudentEntity
            {
                Id = user.Id,
                EnrollmentYear = command.EnrollmentYear.Value,
                User = user,
                ThesisSubject = null
            };

            var added = await _phdStudentRepository.AddAsync(phd);
            if (!added)
                return Result<bool>.FailureResult("Failed to create PhD student profile");

            user.Role = UserRole.PhDStudent;
            var updated = await _userRepository.UpdateUserAsync(user);
            return updated ? Result<bool>.SuccessResult(true) : Result<bool>.FailureResult("Failed to update user role");
        }

        private async Task<Result<bool>> AssignMasterianRoleAsync(UserEntity user, UpdateUserRoleCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Cohort) || string.IsNullOrWhiteSpace(command.DissertationSubject))
                return Result<bool>.FailureResult("Cohort and DissertationSubject are required for Masterian role");

            var master = new MasterianEntity
            {
                Id = user.Id,
                Cohort = command.Cohort!.Trim(),
                DissertationSubject = command.DissertationSubject!.Trim(),
                User = user
            };

            var added = await _masterianRepository.AddAsync(master);
            if (!added)
                return Result<bool>.FailureResult("Failed to create Masterian profile");

            user.Role = UserRole.Masterian;
            var updated = await _userRepository.UpdateUserAsync(user);
            return updated ? Result<bool>.SuccessResult(true) : Result<bool>.FailureResult("Failed to update user role");
        }

        private async Task DeleteExistingProfileAsync(Guid userId, UserRole currentRole)
        {
            switch (currentRole)
            {
                case UserRole.Researcher:
                    var researcher = await _researcherRepository.GetByUserIdAsync(userId);
                    if (researcher != null) await _researcherRepository.DeleteAsync(researcher);
                    break;

                case UserRole.PhDStudent:
                    var phd = await _phdStudentRepository.GetByUserIdAsync(userId);
                    if (phd != null) await _phdStudentRepository.DeleteAsync(phd);
                    break;

                case UserRole.Masterian:
                    var masterian = await _masterianRepository.GetByUserIdAsync(userId);
                    if (masterian != null) await _masterianRepository.DeleteAsync(masterian);
                    break;
            }
        }
    }
}
