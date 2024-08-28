using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using SelfGuidedTours.Core.Contracts.BlobStorage;
using static SelfGuidedTours.Common.ValidationConstants.BlobStorageConstants;

namespace SelfGuidedTours.Core.Services.BlobStorage
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient blobServiceClient;
        public BlobService(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
        }

        public async Task DeleteFileAsync(string blobName, string containerName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }

        public string GetFileUrl(string blobName, string containerName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = containerClient.GetBlobClient(blobName);

            return blobClient.Uri.ToString();
        }

        public async Task<string> UploadFileAsync(string containerName, IFormFile file, string blobName, bool isThumbnail = false)
        {

            ValidateFileUpload(file, isThumbnail); // add isThumbnail flag so that user wont upload video for thumbnail

            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();

            using (var stream = file.OpenReadStream())
            {
                await containerClient.UploadBlobAsync(blobName, stream);
            }

            var httpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.SetHttpHeadersAsync(httpHeaders);

            return blobClient.Uri.AbsoluteUri;
        }
        private void ValidateFileUpload(IFormFile request, bool isThumbnail = false)
        {
            var allowedExtensions = AllowedExtensions; // add all file extensions 
            long maxFileSizeInBytes = MaxFileSyzeInBytes; // 100 megabytes 

            if (isThumbnail)
            {
                allowedExtensions = AllowedThumbnailExtensions;
                maxFileSizeInBytes = MaxThumbnailFileSyzeInBytes; // 10 megabytes 
            }

            if (allowedExtensions.Contains(Path.GetExtension(request.FileName.ToLower())) == false)
            {
                throw new ArgumentException($"Invalid file extension, valid extensions ({string.Join(", ", allowedExtensions)})");
            }
            if (request.Length > maxFileSizeInBytes)
            {
                throw new ArgumentException($"File size is too large, max {ConvertBytesToMegabytes(maxFileSizeInBytes)} mb.");
            }

        }
        private double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        public async Task<string> GetEmailTemplateAsync(string templateUrl)
        {
            var blobClient =  new BlobClient(new Uri(templateUrl));

            if(await blobClient.ExistsAsync())
            {
                var downloadedTemplate = await blobClient.DownloadContentAsync();
            
                return downloadedTemplate.Value.Content.ToString();
            }
            else
            {
                throw new FileNotFoundException($"Email template not found at URL: {templateUrl}");
            }

            
        }
    }
}
