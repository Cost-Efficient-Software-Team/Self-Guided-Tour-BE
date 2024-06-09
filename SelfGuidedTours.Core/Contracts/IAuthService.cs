using SelfGuidedTours.Core.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterInputModel model);
    }
}
