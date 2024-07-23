using Microsoft.EntityFrameworkCore.ChangeTracking;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Infrastructure.Common
{
    public interface IRepository
    {
        IQueryable<T> All<T>() where T : class;
        IQueryable<T> AllReadOnly<T>() where T : class;
        Task AddAsync<T>(T entity) where T : class;
        EntityEntry Delete<T>(T entity) where T : class;
        Task DeleteAllAsync<T>(IEnumerable<T> entities) where T : class;
        Task<int> SaveChangesAsync();
        Task UpdateAsync<T>(T entity) where T : class;
        Task<T?> GetByIdAsync<T>(object id) where T : class;
        Task<UserProfile?> GetProfileAsync(Guid userId);
        Task<UserProfile?> UpdateProfileAsync(Guid userId, UserProfile profile);
    }
}
