using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Infrastructure.Data
{
    public class SelfGuidedToursDbContext:IdentityDbContext
    {
        public SelfGuidedToursDbContext(DbContextOptions<SelfGuidedToursDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //TODO: move these in sepparate configuration classes
            modelBuilder.Entity<TourLandmark>()
                .HasKey(tl => new { tl.TourId, tl.LandmarkId });

            modelBuilder.Entity<Payment>()
           .HasOne(p => p.Tour)
           .WithMany(t => t.Payments)
           .HasForeignKey(p => p.TourId)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
            .HasOne(r => r.Tour)
            .WithMany(t => t.Reviews)
            .HasForeignKey(r => r.TourId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserTours>()
           .HasOne(ut => ut.Tour)
           .WithMany(t => t.UserTours)
           .HasForeignKey(ut => ut.TourId)
           .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
            //Add random GUID strings for the role ids
            var userRoleId = "4f8554d2-cfaa-44b5-90ce-e883c804ae90";
            var adminRoleId = "656a6079-ec9a-4a98-a484-2d1752156d60";

            //Create User and Admin roles
            var roles = new List<IdentityRole>()
            {
                new IdentityRole
                {
                    Id = userRoleId,
                    ConcurrencyStamp = userRoleId,
                    Name = "User",
                    NormalizedName = "USER",
                },
                new IdentityRole
                {
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                }
            };
            // Seed the roles in the Database if they do not exist
            modelBuilder.Entity<IdentityRole>().HasData(roles); 
        }

        public DbSet<Coordinate> Coordinates { get; set; }
        public DbSet<Landmark> Landmarks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourLandmark> TourLandmarks { get; set; }
        public DbSet<UserTours> UserTours { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


    }
}
