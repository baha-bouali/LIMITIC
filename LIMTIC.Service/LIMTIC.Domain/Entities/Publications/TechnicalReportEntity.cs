using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Publications
{
    public class TechnicalReportEntity : BaseEntity
    {
        public Guid PublicationId { get; set; }         
        public PublicationEntity Publication { get; set; }

        public long ReportNumber { get; set; }        
        public string Institution { get; set; }         
    }
}
