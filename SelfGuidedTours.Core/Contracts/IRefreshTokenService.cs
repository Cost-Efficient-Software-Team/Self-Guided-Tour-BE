using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken?> GetByTokenAsync(string refreshToken);
        Task CreateAsync(RefreshToken token);
        Task DeleteAsync(Guid id);
        Task DeleteAllAsync(string userId);
    }
}
