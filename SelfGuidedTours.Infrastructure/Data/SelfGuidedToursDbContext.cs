using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Infrastructure.Data.Models;
using SelfGuidedTours.Infrastructure.Data.SeedDb;

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
            //modelBuilder.Entity<TourLandmark>()
            //    .HasKey(tl => new { tl.TourId, tl.LandmarkId });
            modelBuilder.ApplyConfiguration(new TourLandmarkConfiguration());
           // modelBuilder.Entity<Payment>()
           //.HasOne(p => p.Tour)
           //.WithMany(t => t.Payments)
           //.HasForeignKey(p => p.TourId)
           //.OnDelete(DeleteBehavior.Restrict);
           modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            //modelBuilder.Entity<Review>()
            //.HasOne(r => r.Tour)
            //.WithMany(t => t.Reviews)
            //.HasForeignKey(r => r.TourId)
            //.OnDelete(DeleteBehavior.Restrict);
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            // modelBuilder.Entity<UserTours>()
            //.HasOne(ut => ut.Tour)
            //.WithMany(t => t.UserTours)
            //.HasForeignKey(ut => ut.TourId)
            //.OnDelete(DeleteBehavior.Restrict);
            modelBuilder.ApplyConfiguration(new UserToursConfiguration());
            modelBuilder.ApplyConfiguration(new RolesConfiguration());
            modelBuilder.ApplyConfiguration(new UsersConfiguration());
            modelBuilder.ApplyConfiguration(new UsersRolesConfiguration());
            base.OnModelCreating(modelBuilder);
            
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
