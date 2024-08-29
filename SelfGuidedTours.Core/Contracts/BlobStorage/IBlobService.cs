using Microsoft.AspNetCore.Http;

namespace SelfGuidedTours.Core.Contracts.BlobStorage
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(string containerName, IFormFile file, string blobName, bool isThubmnail = false);
        Task DeleteFileAsync(string blobName, string containerName);
        string GetFileUrl(string blobName, string containerName);   
        Task<string> GetEmailTemplateAsync(string templateUrl);
    }
}
