using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.AccountDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace MuseumSystem.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/dashboards")]
    [ApiController]
    public class DashboardAdmin : ControllerBase
    {
        private readonly IDashboardAdminService dashboardAdminService;
        public DashboardAdmin(IDashboardAdminService dashboardAdminService)
        {
            this.dashboardAdminService = dashboardAdminService;
        }

        [HttpGet("artifact-stats")]
        [SwaggerOperation(
            Summary = "Get artifact statistics",
            Description = "Retrieves statistics related to artifacts for a specific museum.")]
        public async Task<IActionResult> GetArtifactStatsAsync()
        {
            var result = await dashboardAdminService.GetArtifactStatsAsync();
            return Ok(ApiResponse<object>.OkResponse(result, "Artifact stats retrieved successfully", "200"));
        }

        [HttpGet("staff-stats")]
        [SwaggerOperation(
            Summary = "Get staff statistics",
            Description = "Retrieves statistics related to staff for a specific museum.")]
        public async Task<IActionResult> GetStaffStatsAsync()
        {
            var result = await dashboardAdminService.GetStaffStatsAsync();
            return Ok(ApiResponse<object>.OkResponse(result, "Staff stats retrieved successfully", "200"));
        }
    }
}
