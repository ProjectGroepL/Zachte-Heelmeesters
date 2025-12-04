using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;     
using ZhmApi.Models;   
using ZhmApi.Data;

namespace ZhmApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly ApiContext _context;

        public AppointmentsController(ApiContext context)
        {
            _context = context;
        }

        // POST: api/appointments
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
        {
            // mandatory referral check
            var referralExists = await _context.Referrals
                .AnyAsync(r => r.Id == appointment.ReferralId);

            if (!referralExists)
            {
                return BadRequest(new { message = "Referral does not exist." });
            }

            // save to database
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok(appointment);
        }

        // GET: api/appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            return await _context.Appointments
                .Include(a => a.Referral)
                .ToListAsync();
        }
    }

}