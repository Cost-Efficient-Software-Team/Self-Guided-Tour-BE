using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Infrastructure.Data;

namespace SelfGuidedTours.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly SelfGuidedToursDbContext context;

        public TestController(SelfGuidedToursDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public  IActionResult Get()
        {
            var tours = context.Tours;
            return Ok(tours);
        }



    }
}
