using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Dtos.AccountDtos;
using MuseumSystem.Application.Dtos.AuthDtos;
using MuseumSystem.Application.Exceptions;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;
using System.Security.Authentication;

namespace MuseumSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<AuthService> _logger;
        private readonly IGenerateTokenService _generateTokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRedisCacheService _redisCacheService;

        public AuthService(
            IUnitOfWork unit,
            ILogger<AuthService> logger,
            IConfiguration configuration,
            IGenerateTokenService generateTokenService,
            IRedisCacheService redisCacheService,
            ICurrentUserService currentUserService)
        {
            _unit = unit;
            _logger = logger;
            _generateTokenService = generateTokenService;
            _redisCacheService = redisCacheService;
            _currentUserService = currentUserService;
        }

        public async Task<UserProfileResponse> GetCurrentUserProfileAsync()
        {
            var currentUserId = _currentUserService.UserId;
            var account = await _unit.GetRepository<Account>()
                .FindAsync(x => x.Id == currentUserId,
                include: source => source
                .Include(x => x.Museum)
                .Include(x => x.Role))
                ?? throw new NotFoundException("Current user not found.");
            return new UserProfileResponse
            {

                Id = account.Id,
                Email = account.Email,
                FullName = account.FullName,
                Status = account.Status.ToString(),
                MuseumId = account.MuseumId,
                MuseumName = account.Museum?.Name,
                MuseumDescription = account.Museum?.Description,
                MuseumLocation = account.Museum?.Location,
                RoleId = account.RoleId,
                RoleName = account.Role.Name
            };

        }

        public async Task<AuthResponse> LoginAsync(AuthRequest request)
        {
            if (request.Email == null || request.Password == null)
            {
                throw new ArgumentNullException("Email or Password cannot be null.");
            }
            var accountExisting = await _unit.GetRepository<Account>().FindAsync(
                x => x.Email == request.Email,
                include: source => source
                .Include(x => x.Role));
            if (accountExisting == null)
            {
                throw new UnauthorizedAccessException($"Invalid email or password.");
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, accountExisting.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            if (accountExisting.Status == EnumStatus.Inactive)
            {
                throw new UnauthorizedAccessException("Account is not active.");
            }
            _logger.LogInformation("User with email {Email} logged in successfully.", request.Email);

            return new AuthResponse
            {
                UserId = accountExisting.Id,
                Role = accountExisting.Role.Name,
                MuseumId = accountExisting.MuseumId,
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

        public Task Logout()
        {
            // Take current user id from token
            var currentUserId = _currentUserService.UserId;
            if (currentUserId == null)
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }
            // Remove museumId from redis cache
            return _redisCacheService.RemoveMuseumIdAsync(currentUserId);
        }

        public async Task<AccountRespone> RegisterAccountWithMuseumAsync(RegisterRequest request)
        {
            // Register Museum
            var museum = new Museum
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.MuseumName,
                Location = request.MuseumLocation,
                Description = request.MuseumDescription,
                CreateAt = DateTime.UtcNow,
                Status = EnumStatus.Pending
            };

            await _unit.MuseumRepository.InsertAsync(museum);
            await _unit.SaveChangeAsync();

            var hashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var role = await _unit.GetRepository<Role>().FindAsync(x => x.Name == "Admin")
                ?? throw new NotFoundException("Admin role not found, please contact with support.");

            var account = new Account
            {
                Email = request.Email,
                Password = hashPassword,
                FullName = request.FullName,
                CreateAt = DateTime.UtcNow,
                Status = EnumStatus.Active,
                MuseumId = museum.Id,
                RoleId = role.Id,
            };

            await _unit.AccountRepository.InsertAsync(account);
            await _unit.SaveChangeAsync();

            return new AccountRespone
            {
                Id = account.Id,
                Email = account.Email,
                FullName = account.FullName,
                Status = account.Status,
                CreateAt = account.CreateAt,
                UpdateAt = account.UpdateAt,
                MuseumId = account.MuseumId,
                MuseumName = request.MuseumName,
                RoleId = account.RoleId,
                RoleName = role.Name
            };
        }
    }
}
