using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Contracts.BlobStorage;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Enums;
using SelfGuidedTours.Infrastructure.Data.Models;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
namespace SelfGuidedTours.Core.Services
{
    public class LandmarkResourceService : ILandmarkResourceService
    {
        private readonly IBlobService blobService;
        private readonly IRepository repository;

        public LandmarkResourceService(IBlobService blobService, IRepository repository)
        {
            this.blobService = blobService;
            this.repository = repository;
        }

        public async Task CreateLandmarkResourcesAsync(List<IFormFile> resources, Landmark landmark)
        {
            if (landmark is null) throw new ArgumentException(TourWithNoLandmarksErrorMessage);

            var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME");

            if (containerName is null) throw new Exception(ContainerNameErrorMessage);

            //Think about validating if there are 0 resources, is that okay or not
            foreach (var resource in resources)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(resource.FileName)}";
                var resourceUrl = await blobService.UploadFileAsync(containerName, resource, fileName);

                var landmarkResource = new LandmarkResource
                {
                    Url = resourceUrl,
                    Type = GetResourceType(resource.ContentType),
                    Landmark = landmark
                };

                await repository.AddAsync(landmarkResource);

            }
        }

        public async Task UpdateLandmarkResourcesAsync(List<IFormFile> resources, Landmark landmark)
        {
            var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME")
                                ?? throw new ApplicationException(ContainerNameErrorMessage);

            // Първо изтриваме съществуващите ресурси
            var existingResources = await repository.All<LandmarkResource>()
                .Where(r => r.LandmarkId == landmark.LandmarkId)
                .ToListAsync();

            foreach (var existingResource in existingResources)
            {
                // Изтриваме файла от Blob Storage
                await blobService.DeleteFileAsync(existingResource.Url, containerName);
                // Премахваме ресурса от базата данни
                repository.Delete(existingResource);
            }

            // Добавяме новите ресурси
            foreach (var resourceFile in resources)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(resourceFile.FileName)}";
                var resourceUrl = await blobService.UploadFileAsync(containerName, resourceFile, fileName);

                var resourceType = GetResourceType(resourceFile.ContentType);

                var newResource = new LandmarkResource
                {
                    Url = resourceUrl,
                    Type = resourceType,
                    LandmarkId = landmark.LandmarkId
                };
                await repository.AddAsync(newResource);
            }
        }




        public async Task CreateLandmarkResourcesFromUpdateDtoAsync(List<ResourceUpdateDTO> resourcesDto, Landmark landmark)
        {
            //Here look what im getting:
            //ResourceId - 10
            //ResourceType - null
            //ResourceUrl - null

            //ResourceId- null
            //ResourceType - null
            //ResourceUrl - null

            var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME");

            if (containerName is null) throw new Exception(ContainerNameErrorMessage);

            foreach (var resourceDto in resourcesDto)
            {
                if (resourceDto.ResourceFile != null)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(resourceDto.ResourceFile.FileName)}";
                    var resourceUrl = await blobService.UploadFileAsync(containerName, resourceDto.ResourceFile, fileName);

                    var resourceType = GetResourceType(resourceDto.ResourceFile.ContentType);

                    var newResource = new LandmarkResource
                    {
                        Url = resourceUrl,
                        Type = resourceType,
                        Landmark = landmark
                    };
                    await repository.AddAsync(newResource);
                }
                else if (!string.IsNullOrEmpty(resourceDto.ResourceUrl) && resourceDto.ResourceType.HasValue)
                {
                    var newResource = new LandmarkResource
                    {
                        Url = resourceDto.ResourceUrl,
                        Type = (ResourceType)resourceDto.ResourceType.Value,
                        Landmark = landmark
                    };
                    await repository.AddAsync(newResource);
                }
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
