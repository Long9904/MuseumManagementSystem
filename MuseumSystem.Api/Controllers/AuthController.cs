using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
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
        private readonly IRedisCacheService _redisCacheService;

        public AuthController(
            ILogger<AccountController> logger, 
            IAuthService authService, 
            IRedisCacheService redisCacheService)
        {
            _logger = logger;
            _authService = authService;
            _redisCacheService = redisCacheService;
        }

        [HttpPost("login/google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] LoginGGRequest request)
        {
            var result = await _authService.LoginGoogleAsync(request);
            await _redisCacheService.SetMuseumIdAsync(result.UserId, result.MuseumId);

            var dummy = new {Token = result.Token};

            return Ok(ApiResponse<Object>.OkResponse(dummy,"Get token successful!","200"));
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            var result = await _authService.LoginAsync(request);

            await _redisCacheService.SetMuseumIdAsync(result.UserId, result.MuseumId);

            var dummy = new { Token = result.Token };
            return Ok(ApiResponse<Object>.OkResponse(dummy, "Get token successful!", "200"));
        }
    }
}
