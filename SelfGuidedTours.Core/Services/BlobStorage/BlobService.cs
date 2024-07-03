using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using SelfGuidedTours.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGuidedTours.Core.Services.Storage
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
        }
        

        public async Task<string> UploadFileAsync(string containerName, IFormFile file, string blobName, bool isThumbnail = false)
        {

            ValidateFileUpload(file,isThumbnail); // add isThumbnail flag so that user wont upload video for thumbnail

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
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", "mp4", "mp3", "txt" }; // add all file extensions 
            var maxFileSizeInBytes = 104857600; // 100 megabytes 

            if (isThumbnail)
            {
                allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                maxFileSizeInBytes = 10485760; // 10 megabytes 
            }



            if (allowedExtensions.Contains(Path.GetExtension(request.FileName.ToLower())) == false)
            {
               throw new ArgumentException($"Invalid file extension, valid extensions ({string.Join(", ",allowedExtensions)})");
            }
            if (request.Length > maxFileSizeInBytes)
            {
                throw new ArgumentException( "File size is too large, max 10mb");
            }

        }
    }
}
