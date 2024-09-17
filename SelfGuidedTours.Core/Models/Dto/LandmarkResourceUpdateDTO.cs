using Microsoft.AspNetCore.Http;
using SelfGuidedTours.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace SelfGuidedTours.Core.Models.Dto
{
    public class LandmarkResourceUpdateDTO
    {
        public int LandmarkResourceId { get; set; } // ID на ресурса, който ще се обновява

        // Поддръжка за файлове - ако потребителят качва нов файл
        public IFormFile? ResourceFile { get; set; }

        // Поддръжка за URL адреси - ако ресурсът е URL
        public string? ResourceUrl { get; set; }

        // Тип на ресурса (например: изображение, видео, аудио и т.н.)
        [Required]
        public ResourceType Type { get; set; }
    }
}
