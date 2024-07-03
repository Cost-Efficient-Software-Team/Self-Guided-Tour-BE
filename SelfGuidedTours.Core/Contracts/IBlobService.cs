using Microsoft.AspNetCore.Http;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(string containerName, IFormFile file, string blobName, bool isThubmnail = false);
       
    }
}
