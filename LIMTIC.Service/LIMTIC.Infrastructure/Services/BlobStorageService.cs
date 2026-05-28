using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LIMTIC.Application.Abstractions.Storage;

namespace LIMTIC.Infrastructure.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        }

        public async Task<bool> UploadStreamAsync(Stream stream, string containerName, string destinationPath, bool overwrite)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            var container = GetContainerClient(containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.None);

            var blobName = NormalizeBlobName(destinationPath);
            var blob = container.GetBlobClient(blobName);

            if (!overwrite)
            {
                var exists = await blob.ExistsAsync();
                if (exists.Value)
                    return false;
            }

            try
            {
                if (stream.CanSeek)
                    stream.Position = 0;

                await blob.UploadAsync(stream, overwrite: overwrite);
                return true;
            }
            catch (RequestFailedException ex) when (!overwrite && ex.Status == 409)
            {
                return false;
            }
        }

        public async Task<bool> DeleteBlobAsync(string containerName, string path)
        {
            var container = GetContainerClient(containerName);
            var blobName = NormalizeBlobName(path);
            var blob = container.GetBlobClient(blobName);
            try
            {
                var response = await blob.DeleteIfExistsAsync();
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return false;
            }
        }

        public async Task<Stream> GetStreamAsync(string containerName, string path)
        {
            var container = GetContainerClient(containerName);
            var blobName = NormalizeBlobName(path);
            var blob = container.GetBlobClient(blobName);

            try
            {
                var response = await blob.DownloadStreamingAsync();
                return response.Value.Content;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                throw new FileNotFoundException("Blob not found.", $"{containerName}/{blobName}");
            }
        }

        private BlobContainerClient GetContainerClient(string containerName)
        {
            var normalizedContainer = NormalizeContainerName(containerName);
            return _blobServiceClient.GetBlobContainerClient(normalizedContainer);
        }

        private static string NormalizeContainerName(string containerName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("Container name is required.", nameof(containerName));

            var normalized = containerName.Trim().ToLowerInvariant();

            if (normalized.Contains('/') || normalized.Contains('\\'))
                throw new InvalidOperationException("Invalid container name.");
            if (normalized == "." || normalized == ".." || normalized.Contains(".."))
                throw new InvalidOperationException("Invalid container name.");

            return normalized;
        }

        private static string NormalizeBlobName(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path is required.", nameof(path));

            var normalized = path
                .Replace('\\', '/')
                .TrimStart('/');

            var segments = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0 || segments.Any(s => s == "." || s == ".."))
                throw new InvalidOperationException("Invalid destination path.");

            return string.Join('/', segments);
        }
    }
}

