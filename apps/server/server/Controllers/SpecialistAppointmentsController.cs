using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Dtos;

[Authorize(Roles = "Specialist")]
[ApiController]
[Route("api/specialist/appointments")]
public class SpecialistAppointmentsController : ControllerBase
{

        private readonly ApiContext _context;

        public SpecialistAppointmentsController(

            ApiContext context
        )
        {

            _context = context;
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetMyAppointments()
        {
            var specialistIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(specialistIdClaim, out var specialistId))
                return Unauthorized();

            var appointments = await _context.Appointments
                .Include(a => a.Referral)
                    .ThenInclude(r => r.Patient)
                .Include(a => a.Referral.Treatment)
                .Where(a => a.SpecialistId == specialistId)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    ReferralId = a.ReferralId,
                    Notes = a.Referral.Notes,
                    Status = a.Status,
                    TreatmentDescription = a.Referral.Treatment.Description,
                    TreatmentInstructions = a.Referral.Treatment.Instructions,
                    PatientName =
                        a.Referral.Patient.FirstName + " " +
                        a.Referral.Patient.LastName,
                    Date = a.Date.ToString("yyyy-MM-dd HH:mm")
                })
                .ToListAsync();

            return Ok(appointments);
        }
        }