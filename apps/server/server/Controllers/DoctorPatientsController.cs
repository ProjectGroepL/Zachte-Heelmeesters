using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Models;

namespace ZhmApi.Controllers
{
    [Authorize(Roles = "Huisarts,Specialist")]
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
        public async Task<IActionResult> AddPatientToDoctor(int patientId)
        {
            var doctorId = int.Parse(User.FindFirst("id")!.Value);

            // Check if relationship already exists
            bool exists = await _db.DoctorPatients
                .AnyAsync(dp => dp.DoctorId == doctorId && dp.PatientId == patientId);

            if (exists)
                return Conflict("Patient already gekoppeld.");

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
        public async Task<IActionResult> GetMyPatients()
        {
            var doctorId = int.Parse(User.FindFirst("id")!.Value);

            var patients = await _db.DoctorPatients
                .Where(dp => dp.DoctorId == doctorId)
                .Select(dp => new
                {
                    dp.PatientId,
                    FullName = dp.Patient.FirstName + " " + dp.Patient.LastName
                })
                .ToListAsync();

            return Ok(patients);
        }
    }
}
