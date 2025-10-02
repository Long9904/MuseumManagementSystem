using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuseumSystem.Infrastructure.DatabaseSetting;

namespace MuseumSystem.Api.Controllers
{
    [Route("api/v1/health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HealthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("check-connection")]
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
