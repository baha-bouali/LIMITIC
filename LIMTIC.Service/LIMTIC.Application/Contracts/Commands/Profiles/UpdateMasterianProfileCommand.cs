namespace LIMTIC.Application.Contracts.Commands.Profiles
{
    public class UpdateMasterianProfileCommand
    {
        public Guid UserId { get; set; }
        public string DissertationSubject { get; set; }
        public string Cohort { get; set; }
        public string? PhotoUrl { get; set; }
        public Guid? SupervisorId { get; set; }
    }
}
