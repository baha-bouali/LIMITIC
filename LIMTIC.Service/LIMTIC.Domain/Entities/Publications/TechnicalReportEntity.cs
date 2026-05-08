using LIMTIC.Domain.Shared;

namespace LIMTIC.Domain.Entities.Publications
{
    public class TechnicalReportEntity : PublicationEntity
    {
        public long ReportNumber { get; set; }        
        public string Institution { get; set; }         
    }
}
