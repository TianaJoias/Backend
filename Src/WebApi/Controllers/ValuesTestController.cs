using Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesTestController : ControllerBase
    {
        private readonly IIdentityService _identity;

        public ValuesTestController(IIdentityService identity)
        {
            _identity = identity;
        }


        [Authorize]
        [HttpGet]
        [RequiredScope(SCOPES.USER)]
        public IActionResult Get()
        {
            var userId = _identity.GetUserIdentity();
            return Ok(userId);
        }
    }
}
