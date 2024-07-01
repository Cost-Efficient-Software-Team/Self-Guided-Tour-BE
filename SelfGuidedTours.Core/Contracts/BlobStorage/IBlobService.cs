using Microsoft.AspNetCore.Http;

namespace SelfGuidedTours.Core.Contracts.BlobStorage
{
    public interface IBlobService
    {
        Task UploadFileAsync(IFormFile file, string blobName);
        Task DeleteFileAsync(string blobName);
        string GetFileUrl(string blobName);   
    }
}
