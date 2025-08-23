using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DbPingController : ControllerBase
    {
        private readonly IConfiguration _cfg;
        public DbPingController(IConfiguration cfg) => _cfg = cfg;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cs = _cfg.GetConnectionString("DefaultConnection"); // <-- from user-secrets
            try
            {
                await using var conn = new NpgsqlConnection(cs);
                await conn.OpenAsync();
                await using var cmd = new NpgsqlCommand("SELECT 1", conn);
                var result = await cmd.ExecuteScalarAsync();
                return Ok(new { status = "ok", result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }
    }
}
