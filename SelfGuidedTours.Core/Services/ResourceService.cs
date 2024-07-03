using Microsoft.AspNetCore.Http;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Enums;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Services
{
    public class ResourceService : IResourceService
    {
        private readonly IBlobService blobService;
        private readonly IRepository repository;

        public ResourceService(IBlobService blobService, IRepository repository)
        {
            this.blobService = blobService;
            this.repository = repository;
        }
        public async Task CreateLandmarkResoursecAsync(ICollection<IFormFile> resources, Landmark landmark)
        {
            if (landmark is null ) throw new ArgumentException("Landmark is required");

            var containerName = Environment.GetEnvironmentVariable("RESOURCES_CONTAINER");

            if (containerName is null) throw new Exception("Resource Container is not configured");

            //Think about validating if there are 0 resources, is that okay or not
            foreach (var resource in resources)
            {
                var resourceUrl  = await blobService.UploadFileAsync(containerName, resource, resource.FileName);

                var landmarkResource = new LandmarkResource
                {
                    //LandmarkId = landmarkId,
                    Url = resourceUrl,
                    Type = GetResourceType(resource.ContentType),
                    Landmark = landmark
                };
                    
                await repository.AddAsync(landmarkResource);
                //await repository.SaveChangesAsync();
                // upload resource to blob storage
            }

        }
        private ResourceType GetResourceType(string contentType)
        {
            return contentType switch
            {
                "image/jpeg" => ResourceType.Image,
                "image/png" => ResourceType.Image,
                "video/mp4" => ResourceType.Video,
                "audio/mpeg" => ResourceType.Audio,
                "text/plain" => ResourceType.Text,
                "application/pdf" => ResourceType.Text,
                _ => ResourceType.Unknown
            };
        }
    }
}
