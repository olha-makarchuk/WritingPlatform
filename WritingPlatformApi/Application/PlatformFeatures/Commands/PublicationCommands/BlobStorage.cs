using Application.Interfaces;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Diagnostics;

namespace Application.PlatformFeatures.Commands
{
    public class BlobStorage : IBlobStorage
    {
        private readonly BlobServiceClient _client;
        private readonly string _containerName = "books-in-shop";

        public BlobStorage(BlobStorageConfig config)
        {
            _client = new BlobServiceClient(config.ConnectionString);
        }

        public async Task<bool> FileExistsAsync(string fileName)
        {
            var blobClient = _client.GetBlobContainerClient(_containerName).GetBlobClient(fileName);
            var exists = await blobClient.ExistsAsync();
            if (!exists)
            {
                Debug.WriteLine($"Blob '{fileName}' does not exist in container '{_containerName}'.");
            }
            return exists;
        }

        public async Task PutContextAsync(string filename, Stream stream)
        {
            try
            {
                var blobClient = _client.GetBlobContainerClient(_containerName).GetBlobClient(filename);
                await blobClient.UploadAsync(stream, overwrite: true);
            }
            catch (RequestFailedException ex)
            {
                Debug.WriteLine($"Error uploading blob '{filename}' to container '{_containerName}': {ex.Message}");
                throw;
            }
        }

        public List<int> FindByShop(Guid courseId)
        {
            var results = _client.GetBlobContainerClient(_containerName)
                .GetBlobs(prefix: courseId.ToString("N"))
                .AsPages(default, 1000)
                .SelectMany(dt => dt.Values)
                .Select(bi => int.Parse(bi.Name.Split('_').Last()))
                .ToList();

            return results;
        }

        public async Task<List<string>> GetAllAsync()
        {
            var results = _client.GetBlobContainerClient(_containerName)
                .GetBlobs()
                .Select(blobItem => blobItem.Name)
                .ToList();

            return results;
        }

        public async Task DeleteAsync(string filename)
        {
            var blobClient = _client.GetBlobContainerClient(_containerName).GetBlobClient(filename);

            try
            {
                var exists = await blobClient.ExistsAsync();
                if (exists)
                {
                    await blobClient.DeleteAsync();
                }
                else
                {
                    Debug.WriteLine($"Blob '{filename}' does not exist in container '{_containerName}'.");
                }
            }
            catch (RequestFailedException ex)
            {
                Debug.WriteLine($"Error deleting blob '{filename}' from container '{_containerName}': {ex.Message}");
                throw;
            }
        }
    }
}
