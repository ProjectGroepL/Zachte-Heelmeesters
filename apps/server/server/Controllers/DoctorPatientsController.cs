using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Models;

namespace ZhmApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorPatientsController : ControllerBase
    {
        private readonly ApiContext _db;

        public DoctorPatientsController(ApiContext db)
        {
            _db = db;
        }

        [HttpPost]
        [Authorize(Roles = "Huisarts,Specialist")]
        public async Task<IActionResult> AddPatientToDoctor(int patientId)
        {
            // Read user id claim robustly
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!int.TryParse(idClaim, out var doctorId))
            {
                return Unauthorized("Invalid token claims");
            }

            // Check if relationship already exists
            bool exists = await _db.DoctorPatients
                .AnyAsync(dp => dp.DoctorId == doctorId && dp.PatientId == patientId);

            if (exists)
                return Conflict("Patient is al gekoppeld.");

            var relation = new DoctorPatients
            {
                DoctorId = doctorId,
                PatientId = patientId
            };

            _db.DoctorPatients.Add(relation);
            await _db.SaveChangesAsync();

            return Ok("Patient succesvol gekoppeld aan huisarts/specialist!");
        }

        [HttpGet]
        [Authorize(Roles = "Huisarts,Specialist")]
        public async Task<IActionResult> GetMyPatients()
        {
            // Read user id claim robustly
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!int.TryParse(idClaim, out var doctorId))
            {
                return Unauthorized("Invalid token claims");
            }
            try
            {
                // Include Patient navigation to avoid null navigation during projection
                var patients = await _db.DoctorPatients
                    .Where(dp => dp.DoctorId == doctorId)
                    .Include(dp => dp.Patient)
                    .Select(dp => new
                    {
                        dp.PatientId,
                        FullName = dp.Patient != null
                            ? (dp.Patient.FirstName + " " + dp.Patient.LastName)
                            : "(onbekend)"
                    })
                    .ToListAsync();

                return Ok(patients);
            }
            catch (Exception ex)
            {
                // Return a safer 500 with minimal info (stacktrace will still appear in Development)
                return Problem(detail: ex.Message, title: "Failed to get patients");
            }
        }

        // get endpoint to get all GP's from database
        // GET /api/DoctorPatients/general-practitioners
        [HttpGet("general-practitioners")]
        public async Task<IActionResult> GetDoctors()
        {
            var doctors = await (
                from ur in _db.UserRoles
                join u in _db.Users on ur.UserId equals u.Id
                where ur.RoleId == 3
                select new
                {
                    id = u.Id,
                    fullName =
                        u.FirstName + " " +
                        (u.MiddleName ?? "") + " " +
                        u.LastName
                })
                .ToListAsync();
            return Ok(doctors);
        }
    }
}