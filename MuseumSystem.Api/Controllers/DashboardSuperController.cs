using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace MuseumSystem.Api.Controllers
{
    //[Authorize(Roles = "SuperAdmin")]
    [Route("api/v1/superadmin/dashboard")]
    [ApiController]
    public class DashboardSuperController : ControllerBase
    {
        private readonly IDashboardSuperService dashboardService;
        public DashboardSuperController(IDashboardSuperService dashboardService)
        {
            this.dashboardService = dashboardService;
        }
        
        [HttpGet("accounts")]
        [SwaggerOperation(
            Summary = "Get account statistics",
            Description = "Retrieves various statistics related to user accounts.")]
        public async Task<IActionResult> GetAccountStatsAsync()
        {
            var accountStats = await dashboardService.GetAccountStatsAsync();
            return Ok(ApiResponse<object>.OkResponse(accountStats, "Account stats retrieved successfully", "200"));
        }

        [HttpGet("museums")]
        [SwaggerOperation(
            Summary = "Get museum statistics",
            Description = "Retrieves various statistics related to museums.")]
        public async Task<IActionResult> GetMuseumStatsAsync()
        {
            var museumStats = await dashboardService.GetMuseumStatsAsync();
            return Ok(ApiResponse<object>.OkResponse(museumStats, "Museum stats retrieved successfully", "200"));
        }

        [HttpGet("artifacts")]
        [SwaggerOperation(
            Summary = "Get artifact statistics",
            Description = "Retrieves various statistics related to artifacts.")]
        public async Task<IActionResult> GetArtifactStatsAsync()
        {
            var artifactStats = await dashboardService.GetArtifactStatsAsync();
            return Ok(ApiResponse<object>.OkResponse(artifactStats, "Artifact stats retrieved successfully", "200"));
        }
    }
}
