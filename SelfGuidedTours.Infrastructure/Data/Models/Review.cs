﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
using static SelfGuidedTours.Common.ValidationConstants.ValidationConstants.Review;
namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public int TourId { get; set; }
        [ForeignKey(nameof(TourId))]
        public Tour Tour { get; set; } = null!;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        [StringLength(CommentMaxLength,
                       ErrorMessage = CommentLengthErrorMessage)]
        public string? Comment { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}