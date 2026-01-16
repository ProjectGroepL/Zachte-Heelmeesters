using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Extensions;
using ZhmApi.Services;

namespace ZhmApi.Controllers
{
    [Authorize(Roles = "Huisarts")]
    [ApiController]
    [Route("api/doctor/reports")]
    public class DoctorReportsController : ControllerBase
    {
        private readonly DoctorReportService _service;

        public DoctorReportsController(DoctorReportService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var doctorId = User.GetUserId();
            return Ok(await _service.GetForDoctor(doctorId));
        }
    }
}
