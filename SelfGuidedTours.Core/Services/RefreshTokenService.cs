using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRepository repository;

        public RefreshTokenService(IRepository repository)
        {
            this.repository = repository;
        }
        public async Task CreateAsync(RefreshToken refreshToken)
        {
            refreshToken.Id = Guid.NewGuid();

            await repository.AddAsync(refreshToken);
            await repository.SaveChangesAsync();
        }

        public async Task DeleteAllAsync(string userId)
        {
            var tokens = await repository.All<RefreshToken>()
                .Where(t => t.UserId == userId).ToListAsync();

            await repository.DeleteAllAsync(tokens);
            await repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var token = await repository.All<RefreshToken>().FirstOrDefaultAsync(t => t.Id == id);

            if(token == null)
            {
                throw new ArgumentException("Token is invalid!");
            }

            repository.Delete(token);
            await repository.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await repository.AllReadOnly<RefreshToken>()
                .FirstOrDefaultAsync(r => r.Token == token);
        }
    }
}
