using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Models;
using ZhmApi.Data;
using ZhmApi.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ZhmApi.Controllers
{
    [Authorize(Roles = "Patient")]
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
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreateDto request)
        {
            // Referral ophalen
            var referral = await _context.Referrals
                .FirstOrDefaultAsync(r => r.Id == request.ReferralId);

            if (referral == null)
                return BadRequest(new { message = "Referral does not exist." });

            // Combine date & time
            if (!DateTime.TryParse($"{request.Date} {request.Time}", out var appointmentDateTime))
            {
                return BadRequest(new { message = "Invalid date/time format" });
            }

            var appointment = new Appointment
            {
                ReferralId = request.ReferralId,
                SpecialistId = request.SpecialistId,
                Date = appointmentDateTime,
                Status = AppointmentStatus.PendingAccess
            };

            _context.Appointments.Add(appointment);

            // ✅ Status aanpassen
            referral.Status = "afspraak gemaakt";

            await _context.SaveChangesAsync();

            return Ok(appointment);
        }


        // GET: api/appointments
        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var appointmentsRaw = await _context.Appointments
                .Include(a => a.Referral)
                    .ThenInclude(r => r.Patient)
                .Include(a => a.Referral)
                    .ThenInclude(r => r.Treatment)
                .Where(a => a.Referral.PatientId == userId)
                .ToListAsync();

            var appointments = appointmentsRaw.Select(a => new AppointmentDto
            {
                ReferralId = a.ReferralId,
                Notes = a.Referral.Notes,
                Status = a.Status,
                TreatmentDescription = a.Referral.Treatment?.Description ?? "Onbekende behandeling",
                TreatmentInstructions = a.Referral.Treatment?.Instructions,
                PatientName =
                    $"{a.Referral.Patient.FirstName} {a.Referral.Patient.MiddleName} {a.Referral.Patient.LastName}"
                    .Replace("  ", " ")
                    .Trim(),
                Date = a.Date.ToString("yyyy-MM-dd HH:mm")
            });

            return Ok(new { appointments });
        }

    }
}
