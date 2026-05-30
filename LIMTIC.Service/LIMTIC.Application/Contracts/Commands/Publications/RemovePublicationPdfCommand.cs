namespace LIMTIC.Application.Contracts.Commands.Publications
{
    public class RemovePublicationPdfCommand
    {
        public Guid PublicationId { get; set; }
        public Guid FileId { get; set; }
    }
}
