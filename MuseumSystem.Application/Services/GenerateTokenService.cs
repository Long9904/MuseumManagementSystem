using Microsoft.IdentityModel.Tokens;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MuseumSystem.Application.Services
{
    public class GenerateTokenService : IGenerateTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GenerateTokenService> _logger;
        private readonly IRoleService _roleService;

        public GenerateTokenService(IConfiguration configuration, ILogger<GenerateTokenService> logger, IRoleService role)
        {
            _configuration = configuration;
            _logger = logger;
            _roleService = role;
        }

        public async Task<string> GenerateToken(Account accountExisting)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var role = await _roleService.GetRoleByIdAsync(accountExisting.RoleId);
                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountExisting.Id.ToString()),
                        new Claim(ClaimTypes.Email, accountExisting.Email.ToString()),
                        new Claim(ClaimTypes.Name, accountExisting.FullName ?? string.Empty),
                        new Claim(ClaimTypes.Role, role.Name.ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, accountExisting.Id.ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddHours(12),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescription);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token: {Message}", ex.Message);
                throw new InvalidOperationException("Error generating JWT token.", ex);
            }
        }

        public string GenerateVisitorToken(Visitor visitor)
        {
            try
            {

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, visitor.Id.ToString()),
                        new Claim(ClaimTypes.Name, visitor.Username.ToString()),
                        new Claim(ClaimTypes.Role, "Visitor"),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, visitor.Id.ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddHours(12),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials
                    (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescription);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for Visitor: {Message}", ex.Message);
                throw new InvalidOperationException("Error generating JWT token.", ex);
            }
        }
    }
}

