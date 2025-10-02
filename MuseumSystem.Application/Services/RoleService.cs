using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IUnitOfWork unitOfWork, ILogger<RoleService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Role> AddRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                _logger.LogError("Role name cannot be null or empty.");
                throw new ArgumentException("Role name cannot be null or empty.", nameof(roleName));
            }
            var existingRole = await _unitOfWork.GetRepository<Role>().FindAsync(x => x.Name.ToLower() == roleName.ToLower());
            if (existingRole != null)
            {
                _logger.LogWarning("Role with name {RoleName} already exists.", roleName);
                throw new InvalidOperationException($"Role with name {roleName} already exists.");
            }
            var role = new Role
            {
                Name = roleName
            };
            await _unitOfWork.GetRepository<Role>().InsertAsync(role);
            await _unitOfWork.SaveChangeAsync();
            _logger.LogInformation("Role {RoleName} added successfully.", roleName);
            return role;

        }

        public async Task DeleteRoleAsync(string id)
        {
            var role = GetRoleByIdAsync(id);
            if (role == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found.", id);
                throw new KeyNotFoundException($"Role with ID {id} not found.");
            }
            await _unitOfWork.GetRepository<Role>().DeleteAsync(id);
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            var roles = await _unitOfWork.GetRepository<Role>().GetAllAsync();
            if (!roles.Any())
            {
                _logger.LogInformation("No roles found in the system.");
                throw new InvalidOperationException("No roles found in the system.");
            }
            return roles.ToList();
        }

        public Task<Role?> GetRoleByIdAsync(string id)
        {
            var role = _unitOfWork.GetRepository<Role>().FindAsync(x => x.Id == id);
            if (role == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found.", id);
                throw new KeyNotFoundException($"Role with ID {id} not found.");
            }
            return role;
        }

        public async Task<Role> UpdateRoleAsync(string id, string newRoleName)
        {
            var role = await GetRoleByIdAsync(id);
            if(role == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found.", id);
                throw new KeyNotFoundException($"Role with ID {id} not found.");
            }
            var roleUpdate = new Role
            {
                Name = newRoleName
            };
            await _unitOfWork.GetRepository<Role>().UpdateAsync(roleUpdate);
            await _unitOfWork.SaveChangeAsync();
            _logger.LogInformation("Role with ID {RoleId} updated successfully.", id);
            return roleUpdate;
        }

    }
}
