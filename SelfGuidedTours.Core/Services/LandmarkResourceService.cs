﻿using Microsoft.AspNetCore.Http;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Contracts.BlobStorage;
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
        public async Task CreateLandmarkResoursecAsync(ICollection<IFormFile> resources, Landmark landmark)
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
