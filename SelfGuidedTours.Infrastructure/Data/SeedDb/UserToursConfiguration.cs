using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfGuidedTours.Infrastructure.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SelfGuidedTours.Infrastructure.Data.SeedDb
{
    internal class UserToursConfiguration : IEntityTypeConfiguration<UserTours>
    {
        public void Configure(EntityTypeBuilder<UserTours> builder)
        {
            builder
                 .HasOne(ut => ut.Tour)
                 .WithMany(t => t.UserTours)
                 .HasForeignKey(ut => ut.TourId)
                 .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
