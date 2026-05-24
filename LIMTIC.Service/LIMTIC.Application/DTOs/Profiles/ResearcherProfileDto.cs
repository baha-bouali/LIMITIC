using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.DTOs.Profiles
{
    public class ResearcherProfileDto
    {
        // User fields
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }

        // Researcher-specific fields
        public string Rank { get; set; }
        public string Specialty { get; set; }
        public string Office { get; set; }
        public string PhoneNumber { get; set; }
        public string? Biography { get; set; }
        public string? Photo { get; set; }
        public string? Orcid { get; set; }
        public string? GoogleScholar { get; set; }
        public string? ResearchGate { get; set; }
        public string? LinkedIn { get; set; }
        public List<ResearchAxisDto> ResearchAxes { get; set; } = [];
    }
}
