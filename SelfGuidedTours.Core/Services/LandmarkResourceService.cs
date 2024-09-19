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

        public async Task UpdateLandmarkResourcesAsync(List<ResourceUpdateDTO> resources, Landmark landmark)
        {
            var existingResources = await repository.All<LandmarkResource>()
                .Where(lr => lr.LandmarkId == landmark.LandmarkId)
                .ToListAsync();

            var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME")
                                ?? throw new ApplicationException(ContainerNameErrorMessage);

            // Изтриване на ресурси, които не са в обновения списък
            var resourcesToDelete = existingResources
                .Where(er => !resources.Any(r => r.ResourceId == er.LandmarkResourceId))
                .ToList();

            foreach (var resourceToDelete in resourcesToDelete)
            {
                await blobService.DeleteFileAsync(resourceToDelete.Url, containerName);
                repository.Delete(resourceToDelete);
            }

            // Актуализиране на съществуващи ресурси или добавяне на нови
            foreach (var resourceDto in resources)
            {
                if (resourceDto.ResourceId.HasValue)
                {
                    // Актуализиране на съществуващ ресурс
                    var existingResource = existingResources.FirstOrDefault(er => er.LandmarkResourceId == resourceDto.ResourceId.Value);

                    if (existingResource != null)
                    {
                        existingResource.Url = resourceDto.ResourceUrl ?? existingResource.Url;
                        existingResource.Type = (ResourceType)(resourceDto.ResourceType ?? (int)existingResource.Type);
                        existingResource.UpdatedAt = DateTime.Now;

                        // Ако е предоставен нов файл, обновяваме го
                        if (resourceDto.ResourceFile != null)
                        {
                            await blobService.DeleteFileAsync(existingResource.Url, containerName);

                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(resourceDto.ResourceFile.FileName)}";
                            var resourceUrl = await blobService.UploadFileAsync(containerName, resourceDto.ResourceFile, fileName);

                            existingResource.Url = resourceUrl;
                            existingResource.Type = GetResourceType(resourceDto.ResourceFile.ContentType);
                        }
                    }
                }
                else
                {
                    // Добавяне на нов ресурс
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
        }

        public async Task CreateLandmarkResourcesFromUpdateDtoAsync(List<ResourceUpdateDTO> resourcesDto, Landmark landmark)
        {
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
