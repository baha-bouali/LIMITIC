using LIMTIC.Domain.Entities.Publications;

namespace LIMTIC.Domain.Entities.Files
{
    public class PublicationFileEntity
    {
        public Guid Id { get; set; }

        public Guid PublicationId { get; set; }

        public string OriginalFileName { get; set; } = string.Empty;

        public string BlobFileName { get; set; } = string.Empty;

        public string BlobPath { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long SizeInBytes { get; set; }

        public DateTime UploadedAtUtc { get; set; }

        public PublicationEntity Publication { get; set; } = null!;
    }
}
