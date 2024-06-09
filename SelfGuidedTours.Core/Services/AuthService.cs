using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository repository;

        public AuthService(IRepository repository)
        {
            this.repository = repository;
        }

        private async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            //In that case if the current user is not registered with the provided email yet, the method will return null.

            return await repository.AllReadOnly<ApplicationUser>()
                .FirstOrDefaultAsync(au => au.Email == email);
        }
        public async Task<string> RegisterAsync(RegisterInputModel model)
        {
            if (await GetByEmailAsync(model.Email) != null)
            {
                throw new ArgumentException("User already exists!");
            }

            if (model.Password != model.RepeatPassword)
            {
                throw new ArgumentException("Passwords do not match!");
            }

            var hasher = new PasswordHasher<ApplicationUser>();

            var user = new ApplicationUser
            {
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                Name = model.Name,
                PasswordHash = hasher.HashPassword(null!, model.Password)  // Hash the password
            };

            await repository.AddAsync(user);
            await repository.SaveChangesAsync();

            return "User registered successfully!";
        }
    }
}
