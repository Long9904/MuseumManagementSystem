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
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                return Ok(canConnect ? "Database OK" : "Database NOT OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CheckDb Error] {ex}");
                return StatusCode(500, ex.Message); 
            }
        }
    }
}
