using Microsoft.AspNetCore.Identity;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Infrastructure.Data.SeedDb
{
    internal class SeedData
    {
        public ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();
        public SeedData()
        {
           SeedAdminUser();
        }
        private void SeedAdminUser()
        {
            var hasher = new PasswordHasher<ApplicationUser>();

            var adminUser = new ApplicationUser
            {
                Email = "admin@selfguidedtours.bg",
                NormalizedEmail = "admin@selfguidedtours.bg".ToUpper(),
                Name = "Admin Adminov",
                NormalizedUserName = "Admin Adminov".ToUpper(),
                PasswordHash = hasher.HashPassword(null!, "D01Parola")
            };

            Users.Add(adminUser);                
        }
    }
}
