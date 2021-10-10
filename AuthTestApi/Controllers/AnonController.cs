using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthTestApi.Controllers
{
    /// <summary>
    /// FallbackController allows anonymous access.
    /// This bypasses all other authorization policies.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class AnonController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { User = "Unknown" });
        }
    }
}
