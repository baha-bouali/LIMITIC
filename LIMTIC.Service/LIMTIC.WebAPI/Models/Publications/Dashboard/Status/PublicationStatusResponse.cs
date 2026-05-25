namespace LIMTIC.WebAPI.Models.Publications.Dashboard.Status
{
    public class PublicationStatusResponse
    {
        public string Message { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class PublicationValidatedResponse : PublicationStatusResponse
    {
        public string? ValidatedBy { get; set; }
    }

    public class PublicationRejectedResponse : PublicationStatusResponse
    {
        public string? RejectionReason { get; set; }
    }
}

