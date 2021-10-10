using System.Linq;
using AuthTestApi.AuthSchemes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthTestApi.Controllers
{
    /// <summary>
    /// AuthSchemeController specifies that AuthSchemeOne is required for authorization.
    /// This only allows users that have successfully authenticated against AuthSchemeOne.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = AuthSchemeOneHandler.AuthSchemeOne)]
    public class AuthSchemeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { User = User.Claims.First().Value });
        }
    }
}
