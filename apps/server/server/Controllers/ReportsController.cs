using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Extensions;
using ZhmApi.Services;

namespace ZhmApi.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
       private readonly AppointmentReportService _service;

       public ReportsController(AppointmentReportService service)
        {
            _service = service; 
        }

        [Authorize(Roles = "Specialist,Administratie")]
        [HttpGet("{reportId}/internal")]
        public async Task<IActionResult> GetInternal(int reportId)
        {
            try
            {
                return Ok(await _service.GetInternal(reportId));
            }
            catch (InvalidOperationException e)
            {
                return NotFound(new { message = e.Message });
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("appointment/{AppointmentId}")]
        public async Task<IActionResult> GetForPatient(int appointmentId)
        {
            var patientId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var report = await _service.GetForPatientByAppointment(
                appointmentId,
                patientId
            );
            
            if (report == null)
                return NotFound(new { message = "Geen rapport gevonden" });
            
            return Ok(report);
        }

        [Authorize(Roles = "Specialist")]
        [HttpPost("{reportId}/send-to-admin")]
        public async Task<IActionResult> SendToAdmin(int reportId)
        {
            try
            {
                var specialistId = User.GetUserId();
                await _service.SendToAdmin(reportId, specialistId);
                return NoContent();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}