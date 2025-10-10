using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MuseumSystem.Application.Utils
{
    public class GetCurrentUserLogin
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCurrentUserLogin(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string? UserId
        {
            get
            {
                var httpUser = _httpContextAccessor.HttpContext?.User;
                return httpUser?.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? httpUser?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? throw new UnauthorizedAccessException();
            }
        }
    }
}
