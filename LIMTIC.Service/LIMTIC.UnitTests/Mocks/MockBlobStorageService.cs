using LIMTIC.Application.Abstractions.Storage;

namespace LIMTIC.UnitTests.Mocks
{
    public class MockBlobStorageService : IBlobStorageService
    {
        private readonly Dictionary<string, byte[]> _storage = new();

        public Task<bool> UploadStreamAsync(Stream stream, string containerName, string destinationPath, bool overwrite)
        {
            var key = $"{containerName}/{destinationPath}";

            if (_storage.ContainsKey(key) && !overwrite)
                return Task.FromResult(false);

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                _storage[key] = memoryStream.ToArray();
            }

            return Task.FromResult(true);
        }

        public Task<Stream> GetStreamAsync(string containerName, string path)
        {
            var key = $"{containerName}/{path}";

            if (!_storage.ContainsKey(key))
                throw new FileNotFoundException($"Blob '{key}' not found.");

            var stream = new MemoryStream(_storage[key]);
            return Task.FromResult<Stream>(stream);
        }

        public Task<bool> DeleteBlobAsync(string containerName, string path)
        {
            var key = $"{containerName}/{path}";

            var removed = _storage.Remove(key);

            return Task.FromResult(removed);
        }
    }
}
