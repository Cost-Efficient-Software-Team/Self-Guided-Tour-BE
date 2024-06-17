using SelfGuidedTours.Core.Models.Auth;
using SelfGuidedTours.Core.Models.ExternalLogin;
using SelfGuidedTours.Infrastructure.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IGoogleAuthService
    {
        Task<AuthenticateResponse> GoogleSignIn(GoogleSignInVM model);
    }
}
