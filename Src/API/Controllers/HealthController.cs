using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController(ApplicationDbContext dbContext, ILogger<HealthController> logger)
        : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly ILogger<HealthController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Check database connectivity
                bool canConnect = await _dbContext.Database.CanConnectAsync();

                if (!canConnect)
                {
                    _logger.LogWarning("Database health check failed");
                    return StatusCode(
                        503,
                        new
                        {
                            status = "unhealthy",
                            timestamp = DateTime.UtcNow,
                            checks = new { database = "unhealthy" },
                        }
                    );
                }

                return Ok(
                    new
                    {
                        status = "healthy",
                        timestamp = DateTime.UtcNow,
                        checks = new { database = "healthy" },
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(
                    503,
                    new
                    {
                        status = "unhealthy",
                        timestamp = DateTime.UtcNow,
                        error = ex.Message,
                    }
                );
            }
        }
    }
}
