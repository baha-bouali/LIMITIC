namespace LIMTIC.Domain.Shared
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
