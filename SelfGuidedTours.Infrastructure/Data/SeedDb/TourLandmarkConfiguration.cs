using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Infrastructure.Data.SeedDb
{
    internal class TourLandmarkConfiguration : IEntityTypeConfiguration<TourLandmark>
    {
        public void Configure(EntityTypeBuilder<TourLandmark> builder)
        {
            builder
                 .HasKey(tl => new { tl.TourId, tl.LandmarkId });
        }
    }
}
