using Microsoft.AspNetCore.Identity;

namespace SelfGuidedTours.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}