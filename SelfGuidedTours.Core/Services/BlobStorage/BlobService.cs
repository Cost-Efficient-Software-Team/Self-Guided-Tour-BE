using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using SelfGuidedTours.Core.Contracts.BlobStorage;

namespace SelfGuidedTours.Core.Services.BlobStorage
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly string containerName;
        public BlobService(string connectionString, string containerName)
        {
            blobServiceClient = new BlobServiceClient(connectionString);
            this.containerName = containerName;
        }

        public async Task DeleteFileAsync(string blobName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }

        public string GetFileUrl(string blobName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            
            var blobClient = containerClient.GetBlobClient(blobName);

            return blobClient.Uri.ToString();
        }

        public async Task UploadFileAsync(IFormFile file, string blobName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            
            await containerClient.CreateIfNotExistsAsync();
            
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }
        }
    }
}
