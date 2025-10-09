using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos.AuthDtos;
using MuseumSystem.Application.Interfaces;

namespace MuseumSystem.Api.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAuthService _authService;
        public AuthController(ILogger<AccountController> logger, IAuthService authService)
        {
            _logger = logger;           
            _authService = authService;
        }
        [HttpPost("login/google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] LoginGGRequest request)
        {
            var result = await _authService.LoginGoogleAsync(request);
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
    }
}
