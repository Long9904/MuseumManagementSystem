using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MuseumSystem.Application.Dtos.AuthDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;
        public AuthService(IUnitOfWork unit, ILogger<AuthService> logger)
        {
            _unit = unit;
            _logger = logger;
        }
        public async Task<AuthResponse> LoginAsync(AuthRequest request)
        {
            if (request.Email == null || request.Password == null)
            {
                _logger.LogError("Email or Password cannot be null.");
                throw new ArgumentNullException("Email or Password cannot be null.");
            }
            var accountExisting = await _unit.GetRepository<Account>().FindByConditionAsync(x => x.Email == request.Email);
            if (accountExisting == null)
            {
                _logger.LogWarning("Account with email {Email} not found.", request.Email);
                throw new KeyNotFoundException($"Account with email {request.Email} not found.");
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, accountExisting.Password))
            {
                _logger.LogWarning("Invalid password for email {Email}.", request.Email);
                throw new UnauthorizedAccessException("Invalid password.");
            }
            if (accountExisting.IsActive != EnumActive.Active)
            {
                _logger.LogWarning("Account with email {Email} is not active.", request.Email);
                throw new UnauthorizedAccessException("Account is not active.");
            }
            _logger.LogInformation("User with email {Email} logged in successfully.", request.Email);
            return new AuthResponse
            {
                Token = GenerateJwtToken(accountExisting)
            };
        }

        private string GenerateJwtToken(Account accountExisting)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountExisting.Id.ToString()),
                        new Claim(ClaimTypes.Email, accountExisting.Email.ToString()),
                        new Claim(ClaimTypes.Name, accountExisting.FullName ?? string.Empty),
                        new Claim(ClaimTypes.Role, accountExisting.Role.Name.ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),
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
