using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Infrastructure.DatabaseSetting;

namespace MuseumSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HealthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("check-db")]
        public async Task<IActionResult> CheckDb()
        {
            if (await _context.Database.CanConnectAsync())
            {
                return Ok("✅ Connected to PostgreSQL successfully!");
            }

            return StatusCode(500, "❌ Failed to connect to PostgreSQL!");
        }
    }
}
