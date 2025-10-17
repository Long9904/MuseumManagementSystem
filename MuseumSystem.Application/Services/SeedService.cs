using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;

namespace MuseumSystem.Application.Services
{
    public class SeedService : ISeedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SeedService> _logger;

        public SeedService(IUnitOfWork unitOfWork, ILogger<SeedService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task SeedSuperAdminAsync()
        {
            var email = Environment.GetEnvironmentVariable("SUPERADMIN_EMAIL");
            var password = Environment.GetEnvironmentVariable("SUPERADMIN_PASSWORD");
            var name = Environment.GetEnvironmentVariable("SUPERADMIN_NAME") ?? "Super Admin";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("SUPERADMIN_EMAIL or SUPERADMIN_PASSWORD missing in .env");
                return;
            }

            // Check role is exist
            var role = await _unitOfWork.GetRepository<Role>().FindAsync(r => r.Name == "SuperAdmin");
            if (role == null)
            {
                role = new Role
                {
                    Name = "SuperAdmin"
                };
                await _unitOfWork.GetRepository<Role>().InsertAsync(role);
                await _unitOfWork.SaveChangeAsync();
            }

            // Check super admin account is exist
            var existingAdmin = await _unitOfWork.GetRepository<Account>().FindAsync(a => a.Email == email);
            if (existingAdmin != null)
            {
                _logger.LogInformation("Super admin account with email {Email} already exists.", email);
                return;
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var superAdmin = new Account
            {
                Email = email,
                Password = hashedPassword,
                FullName = name,
                RoleId = role.Id,
                Status = Domain.Enums.EnumStatus.Active,
                CreateAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Account>().InsertAsync(superAdmin);
            await _unitOfWork.SaveChangeAsync();
        }
    }
}
