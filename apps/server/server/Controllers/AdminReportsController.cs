using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Services;

namespace ZhmApi.Controllers
{
    
    [Authorize(Roles = "Administratie")]
    [ApiController]
    [Route("api/admin/reports")]
    public class AdminReportsController : ControllerBase
    {
        private readonly AdminReportService _service;

        public AdminReportsController(AdminReportService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.GetPendingReports());
        }
    }

}