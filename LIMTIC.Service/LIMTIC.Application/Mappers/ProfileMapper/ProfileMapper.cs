using LIMTIC.Application.DTOs.Profiles;
using LIMTIC.Domain.Entities.Users;

namespace LIMTIC.Application.Mappers.ProfileMapper
{
    public class ProfileMapper : IProfileMapper
    {
        public ResearcherProfileDto MapToResearcherProfileDto(ResearcherEntity researcher)
        {
            return new ResearcherProfileDto
            {
                // UserDto base fields
                Id = researcher.Id,
                Email = researcher.User.Email,
                FirstName = researcher.User.FirstName,
                LastName = researcher.User.LastName,
                Role = researcher.User.Role,
                IsActive = researcher.User.IsActive,
                AvatarBlobName = researcher.User.AvatarBlobName,
                // Researcher-specific fields
                Rank = researcher.Rank,
                Specialty = researcher.Specialty,
                Office = researcher.Office,
                PhoneNumber = researcher.PhoneNumber,
                Biography = researcher.Biography,
                Orcid = researcher.Orcid,
                GoogleScholar = researcher.GoogleScholar,
                ResearchGate = researcher.ResearchGate,
                LinkedIn = researcher.LinkedIn,
                ResearchAxes = SafeMapResearchAxes(researcher)
            };
        }

        private List<ResearchAxisDto> SafeMapResearchAxes(ResearcherEntity researcher)
        {
            try
            {
                return researcher.ResearchAxes
                    .Select(a => new ResearchAxisDto { Id = a.Id, Title = a.Title })
                    .ToList();
            }
            catch
            {
                // If the navigation or underlying join table is missing, return empty list
                return new List<ResearchAxisDto>();
            }
        }

        public PhDStudentProfileDto MapToPhDStudentProfileDto(PhDStudentEntity phDStudent)
        {
            var supervisorName = phDStudent.Supervisor is not null
                ? $"{phDStudent.Supervisor.User.FirstName} {phDStudent.Supervisor.User.LastName}"
                : null;

            return new PhDStudentProfileDto
            {
                // UserDto base fields
                Id = phDStudent.Id,
                Email = phDStudent.User.Email,
                FirstName = phDStudent.User.FirstName,
                LastName = phDStudent.User.LastName,
                Role = phDStudent.User.Role,
                IsActive = phDStudent.User.IsActive,
                AvatarBlobName = phDStudent.User.AvatarBlobName,
                // PhDStudent-specific fields
                ThesisSubject = phDStudent.ThesisSubject,
                EnrollmentYear = phDStudent.EnrollmentYear,
                SupervisorId = phDStudent.SupervisorId,
                SupervisorName = supervisorName,
                ResearchAxes = phDStudent.ResearchAxes
                    .Select(a => new ResearchAxisDto { Id = a.Id, Title = a.Title })
                    .ToList()
            };
        }

        public MasterianProfileDto MapToMasterianProfileDto(MasterianEntity masterian)
        {
            var supervisorName = masterian.Supervisor is not null
                ? $"{masterian.Supervisor.User.FirstName} {masterian.Supervisor.User.LastName}"
                : null;

            return new MasterianProfileDto
            {
                // UserDto base fields
                Id = masterian.Id,
                Email = masterian.User.Email,
                FirstName = masterian.User.FirstName,
                LastName = masterian.User.LastName,
                Role = masterian.User.Role,
                IsActive = masterian.User.IsActive,
                AvatarBlobName = masterian.User.AvatarBlobName,
                // Masterian-specific fields
                DissertationSubject = masterian.DissertationSubject,
                Cohort = masterian.Cohort,
                SupervisorId = masterian.SupervisorId,
                SupervisorName = supervisorName
            };
        }
    }
}
