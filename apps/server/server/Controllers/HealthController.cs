using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Dtos;

namespace ZhmApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private ApiContext _apiContext;

        public HealthController(ApiContext apiContext)
        {
            _apiContext = apiContext;
        }

        [HttpGet]
        public IActionResult GetHealth()
        {
            var databaseHealthy = "Unhealthy";

            try
            {
                // Simple query to check database connectivity
                _apiContext.Database.ExecuteSqlRaw("SELECT 1");
                databaseHealthy = "Healthy";
            }
            catch { }

            var response = new HealthResponse
            {
                Api = "Healthy",
                Database = databaseHealthy,
                Timestamp = DateTime.UtcNow
            };

            return Ok(response);
        }
    }
}
