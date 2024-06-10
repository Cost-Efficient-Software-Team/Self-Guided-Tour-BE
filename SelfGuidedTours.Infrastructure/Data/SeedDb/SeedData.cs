using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Infrastructure.Data.SeedDb
{
    internal class SeedData
    {
        public ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();
        public ICollection<IdentityRole> Roles { get; set; } = new HashSet<IdentityRole>();
        public ICollection<IdentityUserRole<string>> UsersRoles { get; set; } = new HashSet<IdentityUserRole<string>>();
        public SeedData()
        {
            SeedAdminUser();
            SeedRoles();
            SeedUsersRoles();
        }
        private void SeedAdminUser()
        {
            var hasher = new PasswordHasher<ApplicationUser>();

            var adminUser = new ApplicationUser
            {
                Id = "27d78708-8671-4b05-bd5e-17aa91392224",
                Email = "admin@selfguidedtours.bg",
                NormalizedEmail = "admin@selfguidedtours.bg".ToUpper(),
                Name = "Admin Adminov",
                NormalizedUserName = "Admin Adminov".ToUpper(),
                PasswordHash = hasher.HashPassword(null!, "D01Parola")
            };

            Users.Add(adminUser);
        }
        private void SeedRoles()
        {
            //Add random GUID strings for the role ids
            var userRoleId = "4f8554d2-cfaa-44b5-90ce-e883c804ae90";
            var adminRoleId = "656a6079-ec9a-4a98-a484-2d1752156d60";

            //Create User and Admin roles

            // Seed the roles in the Database if they do not exist
            Roles.Add(new IdentityRole
            {
                Id = userRoleId,
                ConcurrencyStamp = userRoleId,
                Name = "User",
                NormalizedName = "USER",
            });
            Roles.Add(new IdentityRole
            {
                Id = adminRoleId,
                ConcurrencyStamp = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
            });

        }
        private void SeedUsersRoles()
        {
            UsersRoles.Add(new IdentityUserRole<string>
            {
                UserId = "27d78708-8671-4b05-bd5e-17aa91392224",
                RoleId = "656a6079-ec9a-4a98-a484-2d1752156d60"
            });
        }
    }
}
