namespace LIMTIC.WebAPI.Models.Publications.Dashboard.CreatePublication
{
    public class CreateDashboardPublicationResponse
    {
        public string Message { get; set; } = string.Empty;
        public CreatedPublicationRef Publication { get; set; } = new();
    }

    public class CreatedPublicationRef
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

