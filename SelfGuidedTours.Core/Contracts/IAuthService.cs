using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterInputModel model);
    }
}
