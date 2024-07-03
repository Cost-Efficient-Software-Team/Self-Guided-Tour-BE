using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SelfGuidedTours.Infrastructure.Data;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Infrastructure.Common
{
    public class Repository : IRepository
    {
        private readonly SelfGuidedToursDbContext context;

        public Repository(SelfGuidedToursDbContext context)
        {
            this.context = context;
        }

        private DbSet<T> DbSet<T>() where T : class => context.Set<T>();

        public async Task AddAsync<T>(T entity) where T : class
        {
            await context.AddAsync(entity);
        }

        public IQueryable<T> All<T>() where T : class
        {
            return DbSet<T>();
        }

        public IQueryable<T> AllReadOnly<T>() where T : class
        {
            return DbSet<T>().AsNoTracking();
        }

        public EntityEntry Delete<T>(T entity) where T : class
        {
            return context.Remove(entity);
        }

        public async Task<T?> GetByIdAsync<T>(object id) where T : class
        {
            return await DbSet<T>().FindAsync(id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public async Task UpdateAsync<T>(T entity) where T : class
        {
            context.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAllAsync<T>(IEnumerable<T> entities) where T : class
        {
            context.RemoveRange(entities);
            await Task.CompletedTask;
        }

        public async Task<UserProfile?> GetProfileAsync(Guid userId)
        {
            return await context.UserProfiles.FindAsync(userId);
        }

        public async Task<UserProfile?> UpdateProfileAsync(Guid userId, UserProfile profile)
        {
            var existingProfile = await context.UserProfiles.FindAsync(userId);
            if (existingProfile == null)
            {
                return null;
            }

            existingProfile.Name = profile.Name;
            existingProfile.Email = profile.Email;

            await context.SaveChangesAsync();
            return existingProfile;
        }
    }
}
