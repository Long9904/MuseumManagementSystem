using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MuseumSystem.Application.Interfaces;

namespace MuseumSystem.Application.Utils
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
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
                    ?? throw new UnauthorizedAccessException("User is not login");
            }
        }

        public string? MuseumId
        {
            get 
            {
                var context = _httpContextAccessor.HttpContext;
                return context?.Items["MuseumId"]?.ToString() 
                    ?? throw new UnauthorizedAccessException("User is not login or not relation with any museum");
            }
        }
    }
}
