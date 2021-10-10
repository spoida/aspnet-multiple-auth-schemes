using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace AuthTestApi.Controllers
{
    /// <summary>
    /// FallbackController doesn't specify an Authorize attribute.
    /// This causes the FallbackPolicy to run.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FallbackController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { User = User.Claims.First().Value });
        }
    }
}
