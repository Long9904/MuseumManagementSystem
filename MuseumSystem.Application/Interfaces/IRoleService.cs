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
        Task<List<Role>> GetAllRolesAsync();
        Task<Role?> GetRoleByIdAsync(string id);
        Task<Role> AddRoleAsync(string roleName);
        Task<Role> UpdateRoleAsync(string id, string newRoleName);
        Task DeleteRoleAsync(string id);
    }
}
