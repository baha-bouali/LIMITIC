using LIMTIC.Application.DTOs.Profiles;
using LIMTIC.Domain.Entities.Users;

namespace LIMTIC.Application.Mappers.ProfileMapper
{
    public interface IProfileMapper
    {
        ResearcherProfileDto MapToResearcherProfileDto(ResearcherEntity researcher);
        PhDStudentProfileDto MapToPhDStudentProfileDto(PhDStudentEntity phDStudent);
        MasterianProfileDto MapToMasterianProfileDto(MasterianEntity masterian);
    }
}
