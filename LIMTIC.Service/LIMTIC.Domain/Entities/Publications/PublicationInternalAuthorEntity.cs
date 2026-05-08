using LIMTIC.Domain.Entities.Users;

namespace LIMTIC.Domain.Entities.Publications
{
    public class PublicationInternalAuthorEntity
    {
        public Guid PublicationId { get; set; }
        public PublicationEntity Publication { get; set; } = null!;

        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;

        public int AuthorOrder { get; set; }
    }
}