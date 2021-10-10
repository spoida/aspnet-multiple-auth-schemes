using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthTestApi.Controllers
{
    /// <summary>
    /// DefaultController specifies an empty [Authorize] attribute.
    /// Normally this would cause the DefaultPolicy to run, but we have explicitly NOT set
    /// a DefaultPolicy. However we HAVE set a FallbackPolicy.
    /// Therefore requests that satisfy the FallbackPolicy will succeed.
    /// Our FallbackPolicy requires AuthSchemeTwo so requests that successfully authenticate
    /// via AuthSchemeTwo will succeed, and all others with result in 401.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class DefaultController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { User = User.Claims.First().Value });
        }
    }
}
