namespace LIMTIC.Application.Abstractions.Storage
{
    public interface IBlobStorageService
    {
        public Task<bool> UploadStreamAsync(Stream stream , string containerName, string destinationPath, bool overwrite);
        public Task<bool> DeleteBlobAsync(string containerName, string path); 
        public Task<Stream> GetStreamAsync(string containerName, string path);
    }
}
