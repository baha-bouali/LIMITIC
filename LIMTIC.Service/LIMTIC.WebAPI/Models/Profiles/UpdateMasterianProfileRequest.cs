namespace LIMTIC.WebAPI.Models.Profiles
{
    public class UpdateMasterianProfileRequest
    {
        public string DissertationSubject { get; set; }
        public string Cohort { get; set; }
        public Guid? SupervisorId { get; set; }
    }
}
