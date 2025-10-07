using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Dtos.RoleDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public async Task<Role> AddRoleAsync(RoleRequest roleRequest)
        {
            if (string.IsNullOrWhiteSpace(roleRequest.Name))
            {
                _logger.LogError("Role name cannot be null or empty.");
                throw new ArgumentException("Role name cannot be null or empty.", nameof(roleRequest.Name));
            }
            if (!Regex.IsMatch(roleRequest.Name, @"^[a-zA-Z0-9 ]+$"))
            {
                _logger.LogError("Role name contains invalid characters.");
                throw new ArgumentException("Role name contains invalid characters.", nameof(roleRequest.Name));
            }
            var existingRole = await _unitOfWork.GetRepository<Role>().FindAsync(x => x.Name == roleRequest.Name);
            if (existingRole != null)
            {
                _logger.LogWarning("Role with name {RoleName} already exists.", roleRequest.Name);
                throw new InvalidOperationException($"Role with name {roleRequest.Name} already exists.");
            }
            var role = new Role
            {
                Name = roleRequest.Name
            };
            await _unitOfWork.GetRepository<Role>().InsertAsync(role);
            await _unitOfWork.SaveChangeAsync();
            _logger.LogInformation("Role {RoleName} added successfully.", roleRequest.Name);
            return role;

        }

        public async Task DeleteRoleAsync(string id)
        {
            var role = await GetRoleByIdAsync(id);
            if (role == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found.", id);
                throw new KeyNotFoundException($"Role with ID {id} not found.");
            }
            await _unitOfWork.GetRepository<Role>().DeleteAsync(id);
        }

        public async Task<BasePaginatedList<Role>> GetAllRolesAsync(int pageIndex , int pageSize )
        {
            var query = _unitOfWork.GetRepository<Role>().Entity;
            return await _unitOfWork.GetRepository<Role>().GetPagging(query, pageIndex, pageSize);
        }

        public async Task<Role?> GetRoleByIdAsync(string id)
        {
            var role = await _unitOfWork.GetRepository<Role>().FindAsync(x => x.Id == id);
            if (role == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found.", id);
                throw new KeyNotFoundException($"Role with ID {id} not found.");
            }
            return role;
        }

        public async Task<Role> UpdateRoleAsync(string id, RoleRequest roleRequest)
        {

            var roleExisting = await GetRoleByIdAsync(id);
            if (roleExisting == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found.", id);
                throw new KeyNotFoundException($"Role with ID {id} not found.");
            }
            var roleWithSameName = await _unitOfWork.GetRepository<Role>().FindAsync(x => x.Name == roleRequest.Name);
            if (roleWithSameName != null)
            {
                _logger.LogWarning("Role with name {RoleName} already exists.", roleRequest.Name);
                throw new InvalidOperationException($"Role with name {roleRequest.Name} already exists.");
            }
            bool isUpdate = false;
            if (roleExisting.Name != roleRequest.Name && string.IsNullOrWhiteSpace(roleRequest.Name))
            {
                roleExisting.Name = roleRequest.Name;
                isUpdate = true;
            }
            if (!isUpdate)
            {
                _logger.LogInformation("No changes detected for Role with ID {RoleId}. Update operation skipped.", id);
                return roleExisting;
            }
            await _unitOfWork.GetRepository<Role>().UpdateAsync(roleExisting);
            await _unitOfWork.SaveChangeAsync();
            _logger.LogInformation("Role with ID {RoleId} updated successfully.", id);
            return roleExisting;
        }

    }
}
