using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
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
                        Email = dp.Patient != null ? dp.Patient.Email : "(onbekend)",
                        FullName = dp.Patient != null
                            ? $"{dp.Patient.FirstName} {(string.IsNullOrEmpty(dp.Patient.MiddleName) ? "" : dp.Patient.MiddleName + " ")}{dp.Patient.LastName}".Trim()
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