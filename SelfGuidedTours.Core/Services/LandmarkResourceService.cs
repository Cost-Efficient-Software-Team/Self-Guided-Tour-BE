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
        public async Task CreateLandmarkResourcesAsync(ICollection<IFormFile> resources, Landmark landmark)
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

        public async Task UpdateLandmarkResourcesAsync(List<LandmarkResourceUpdateDTO> resourcesDto, Landmark landmark)
        {
            // Съществуващи ресурси за обновяване
            var existingResources = await repository.All<LandmarkResource>()
                                    .Where(lr => lr.LandmarkId == landmark.LandmarkId)
                                    .ToListAsync();

            foreach (var resourceDto in resourcesDto)
            {
                var existingResource = existingResources.FirstOrDefault(r => r.LandmarkResourceId == resourceDto.LandmarkResourceId);

                if (existingResource != null)
                {
                    // Ако съществуващ ресурс е намерен, обновяване на URL или файл
                    if (resourceDto.ResourceFile != null)
                    {
                        var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME")
                                            ?? throw new ApplicationException(ContainerNameErrorMessage);

                        await blobService.DeleteFileAsync(existingResource.Url, containerName);

                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(resourceDto.ResourceFile.FileName)}";
                        var newResourceUrl = await blobService.UploadFileAsync(containerName, resourceDto.ResourceFile, fileName, true);

                        existingResource.Url = newResourceUrl;
                    }
                    else if (!string.IsNullOrEmpty(resourceDto.ResourceUrl))
                    {
                        existingResource.Url = resourceDto.ResourceUrl;
                    }

                    existingResource.UpdatedAt = DateTime.Now;
                }
                else
                {
                    // Създаване на нов ресурс, ако не съществува
                    var newResource = new LandmarkResource
                    {
                        LandmarkId = landmark.LandmarkId,
                        Url = resourceDto.ResourceFile != null ?
                                await UploadNewResourceFile(resourceDto.ResourceFile) :
                                resourceDto.ResourceUrl ?? throw new ArgumentNullException(nameof(resourceDto.ResourceUrl)), // Добавяме проверка за null
                        Type = resourceDto.Type,
                        CreatedAt = DateTime.Now
                    };

                    await repository.AddAsync(newResource);
                }
            }

            await repository.SaveChangesAsync();
        }


        private async Task<string> UploadNewResourceFile(IFormFile resourceFile)
        {
            var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME")
                                ?? throw new ApplicationException(ContainerNameErrorMessage);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(resourceFile.FileName)}";
            var resourceUrl = await blobService.UploadFileAsync(containerName, resourceFile, fileName, true);

            return resourceUrl;
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
