using Microsoft.IdentityModel.Tokens;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.CodeDom;

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
                var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
                var role = await _roleService.GetRoleByIdAsync(accountExisting.RoleId);
                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountExisting.Id.ToString()),
                        new Claim(ClaimTypes.Email, accountExisting.Email.ToString()),
                        new Claim(ClaimTypes.Name, accountExisting.FullName ?? string.Empty),
                        new Claim(ClaimTypes.Role, role.Name.ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(12),
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
    }
}
