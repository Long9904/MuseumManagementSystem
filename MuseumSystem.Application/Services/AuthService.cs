using Google.Apis.Auth;
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
using System.Security.Authentication;
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
        private readonly IGenerateTokenService _generateTokenService;

        public AuthService(IUnitOfWork unit, ILogger<AuthService> logger, IConfiguration configuration, IGenerateTokenService generateTokenService)
        {
            _unit = unit;
            _logger = logger;
            _configuration = configuration;
            _generateTokenService = generateTokenService;
        }

        public async Task<AuthResponse> LoginAsync(AuthRequest request)
        {
            if (request.Email == null || request.Password == null)
            {
                throw new ArgumentNullException("Email or Password cannot be null.");
            }
            var accountExisting = await _unit.GetRepository<Account>().FindByConditionAsync(x => x.Email == request.Email);
            if (accountExisting == null)
            {
                throw new KeyNotFoundException($"Account with email {request.Email} not found.");
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, accountExisting.Password))
            {
                throw new UnauthorizedAccessException("Invalid password.");
            }
            if (accountExisting.Status != EnumStatus.Active)
            {
                throw new UnauthorizedAccessException("Account is not active.");
            }
            _logger.LogInformation("User with email {Email} logged in successfully.", request.Email);
            return new AuthResponse
            {
                Token = await _generateTokenService.GenerateToken(accountExisting)
            };
        }

        public async Task<AuthResponse> LoginGoogleAsync(LoginGGRequest loginGGRequest)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(loginGGRequest.IdToken);
                var email = payload.Email;
                var fullName = payload.Name;
                var accountExisting = await _unit.GetRepository<Account>().FindByConditionAsync(x => x.Email == email);
                if (accountExisting == null)
                {
                    throw new KeyNotFoundException($"Account with email {email} not found.");
                }
                return new AuthResponse
                {
                    Token = await _generateTokenService.GenerateToken(accountExisting)
                };
            }
            catch (AuthenticationException ey)
            {
                throw new AuthenticationException($"Error during Google login : {ey.Message}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error during Google login : {ex.Message}");
            }
        }
    }
}
