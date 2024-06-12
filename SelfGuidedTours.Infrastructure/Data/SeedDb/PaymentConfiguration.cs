using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Infrastructure.Data.SeedDb
{
    internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder
                   .HasOne(p => p.Tour)
                   .WithMany(t => t.Payments)
                   .HasForeignKey(p => p.TourId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
