using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuseumSystem.Infrastructure.DatabaseSetting;
using Swashbuckle.AspNetCore.Annotations;

namespace MuseumSystem.Api.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("api/v1/health")]
    [ApiController]
    [SwaggerTag("Health Check - SuperAdmin")]  
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HealthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("check-db-connection")]
        [SwaggerOperation(Summary = "Check Database Connection")]
        public async Task<IActionResult> CheckDb()
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("SELECT 1");
                return Ok("Database OK (Execution Successful)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CheckDb Error] {ex}");
                return StatusCode(500, ex.Message); 
            }
        }
    }
}
