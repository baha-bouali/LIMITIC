using LIMTIC.Application.DTOs.ResearchAxis;
using LIMTIC.Application.DTOs.UserManagement;

namespace LIMTIC.Application.DTOs.Profiles
{
    public class ResearcherProfileDto : UserDto
    {
        public string Rank { get; set; }
        public string Specialty { get; set; }
        public string Office { get; set; }
        public string PhoneNumber { get; set; }
        public string? Biography { get; set; }
        public string? Orcid { get; set; }
        public string? GoogleScholar { get; set; }
        public string? ResearchGate { get; set; }
        public string? LinkedIn { get; set; }
        public List<ResearchAxisDto> ResearchAxes { get; set; } = [];
    }
}
