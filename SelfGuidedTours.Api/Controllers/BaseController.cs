using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected string UserId => User.Identity.IsAuthenticated ? User.Claims.First().Value : string.Empty;
    }
}
