namespace LIMTIC.Application.Contracts.Commands.Publications
{
    public class AddDashboardPublicationPdfCommand
    {
        public Guid Id { get; set; }
        public string PdfUrl { get; set; } = string.Empty;
    }
}

