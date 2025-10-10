
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Utils;

namespace MuseumSystem.Api.Controllers
{
    [Authorize]
    [Route("api/v1/areas")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly GetCurrentUserLogin _getCurrentUserLogin;

        public AreaController(GetCurrentUserLogin getCurrentUserLogin)
        {
            _getCurrentUserLogin = getCurrentUserLogin;
        }

        [HttpGet("current-user")]
        public IActionResult GetCurrentUserId()
        {
            try
            {
                var userId = _getCurrentUserLogin.UserId;
                return Ok(new { UserId = userId });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}
