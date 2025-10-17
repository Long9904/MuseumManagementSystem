using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.RoleDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;

namespace MuseumSystem.Api.Controllers
{
    [Route("api/v1/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IRoleService _roleService;

        public RoleController(ILogger<RoleController> logger, IRoleService roleService)
        {
            _logger = logger;
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageIndex = 1, int pageSize = 10)
        {

            var roles = await _roleService.GetAllRolesAsync(pageIndex, pageSize);
            return Ok(ApiResponse<BasePaginatedList<Role>>.OkResponse(roles, "Get all roles successfully", "200"));

        }

        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] RoleRequest roleRequest)
        {

            var role = await _roleService.AddRoleAsync(roleRequest);
            return Ok(ApiResponse<Role>.OkResponse(role, "Role added successfully", "200"));

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {

            var role = await _roleService.GetRoleByIdAsync(id);
            return Ok(ApiResponse<Role>.OkResponse(role, "Get role by id successfully", "200"));


        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            await _roleService.DeleteRoleAsync(id);
            return Ok(ApiResponse<string>.OkResponse("Role deleted successfully", "200"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleRequest roleRequest)
        {
            var roleUpdate = await _roleService.UpdateRoleAsync(id, roleRequest);
            return Ok(ApiResponse<Role>.OkResponse(roleUpdate, "Role updated successfully", "200"));

        }
    }
}
