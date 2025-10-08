using MuseumSystem.Application.Dtos.RoleDtos;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Interfaces
{
    public interface IRoleService
    {
        Task<BasePaginatedList<Role>> GetAllRolesAsync(int pageIndex , int pageSize);
        Task<Role?> GetRoleByIdAsync(string id);
        Task<Role> AddRoleAsync(RoleRequest request);
        Task<Role> UpdateRoleAsync(string id, RoleRequest roleRequest);
        Task DeleteRoleAsync(string id);
    }
}
