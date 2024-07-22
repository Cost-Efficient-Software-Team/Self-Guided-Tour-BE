using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Infrastructure.Data.Models;
using SelfGuidedTours.Infrastructure.Data.SeedDb;

namespace SelfGuidedTours.Infrastructure.Data
{
    public class SelfGuidedToursDbContext : IdentityDbContext
    {
        public SelfGuidedToursDbContext(DbContextOptions<SelfGuidedToursDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Apply entity configurations
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
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
        public DbSet<UserTours> UserTours { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<LandmarkResource> LandmarkResources { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

    }
}
