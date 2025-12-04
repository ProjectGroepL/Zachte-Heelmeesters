using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Models;
using ZhmApi.Data;
using ZhmApi.Dtos;
using System.Security.Claims;

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
            // Mandatory referral check
            var referralExists = await _context.Referrals
                .AnyAsync(r => r.Id == appointment.ReferralId);

            if (!referralExists)
            {
                return BadRequest(new { message = "Referral does not exist." });
            }

            // Save to database
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok(appointment);
        }

        // GET: api/appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments()
        {
            // Haal userId uit JWT claim van de ingelogde gebruiker
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Niet ingelogd");

            Console.WriteLine($"Logged in userIdClaim: {userId}");

            var appointments = await _context.Appointments
                .Include(a => a.Referral)
                    .ThenInclude(r => r.Patient)
                .Include(a => a.Referral.Treatment)
                .Where(a => a.Referral.PatientId == userId) // Filter op ingelogde user
                .Select(a => new AppointmentDto
                {
                    ReferralId = a.ReferralId,
                    Notes = a.Referral.Notes,
                    Status = a.Referral.Status,
                    TreatmentDescription = a.Referral.Treatment.Description,
                    TreatmentInstructions = a.Referral.Treatment.Instructions,
                    PatientName = $"{a.Referral.Patient.FirstName} {a.Referral.Patient.MiddleName} {a.Referral.Patient.LastName}".Trim()
                })
                .ToListAsync();

            return Ok(new { UserId = userId, Appointments = appointments });

        }
    }
}
