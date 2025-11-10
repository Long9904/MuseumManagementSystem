using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.AccountDtos;
using MuseumSystem.Application.Dtos.AuthDtos;
using MuseumSystem.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace MuseumSystem.Api.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    [SwaggerTag("Authentication Management - Public Access")]
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

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Register account with museum, need admin to confirm")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAccountWithMuseumAsync(request);
            return Ok(ApiResponse<AccountRespone>.OkResponse(result, "Register successful! Please wait for admin confirmation.", "200"));
        }


        [HttpPost("login/google")]
        [SwaggerOperation(Summary = "Login with Google account + credential")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] LoginGGRequest request)
        {
            var result = await _authService.LoginGoogleAsync(request);
            if (result.Role != "SuperAdmin")
            {
                await _redisCacheService.SetMuseumIdAsync(result.UserId, result.MuseumId);
            }

            var dummy = new {Token = result.Token};

            return Ok(ApiResponse<Object>.OkResponse(dummy,"Get token successful!","200"));
        }
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Login with email and password")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            var result = await _authService.LoginAsync(request);

            await _redisCacheService.SetMuseumIdAsync(result.UserId, result.MuseumId);

            var dummy = new { Token = result.Token };
            return Ok(ApiResponse<Object>.OkResponse(dummy, "Get token successful!", "200"));
        }


        [HttpPost("logout")]
        [SwaggerOperation(Summary = "Logout current account")]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout();
            return Ok(ApiResponse<string>.OkResponse("Logout successful", "Logout successful", "200"));
        }

        [HttpGet("profile")]
        [SwaggerOperation(Summary = "Get current user profile")]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            var result = await _authService.GetCurrentUserProfileAsync();
            return Ok(ApiResponse<UserProfileResponse>.OkResponse(result, "Get current user profile successful", "200"));
        }
    }
}
