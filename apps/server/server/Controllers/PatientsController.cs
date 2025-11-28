using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Models;
using ZhmApi.Dtos;

[Route("api/patients")]
[ApiController]
public class PatientsController : ControllerBase
{
    private readonly ApiContext _context;

    public PatientsController(ApiContext context)
    {
        _context = context;
    }

    // GET: api/patients/gps?search=Bob
    [HttpGet("gps")]
    public async Task<ActionResult<IEnumerable<GpDto>>> GetGps([FromQuery] string? search)
    {
        var query = _context.Users
            .Where(u => u.RoleId == 2); // Only GPs

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u =>
                u.FirstName.Contains(search) ||
                (u.MiddleName != null && u.MiddleName.Contains(search)) ||
                u.LastName.Contains(search));
        }

        var gps = await query
            .Select(u => new GpDto
            {
                Id = u.Id,
                FullName = string.IsNullOrEmpty(u.MiddleName)
                    ? $"{u.FirstName} {u.LastName}"
                    : $"{u.FirstName} {u.MiddleName} {u.LastName}"
            })
            .ToListAsync();

        return Ok(gps);
    }

    // PUT: api/patients/2/gp
    [HttpPut("{patientId}/gp")]
    public async Task<IActionResult> SetGeneralPractitioner(int patientId, [FromBody] SetGpDto dto)
    {
        var patient = await _context.Users.FindAsync(patientId);
        if (patient == null || patient.RoleId != 1) // Ensure it's a patient
            return NotFound("Patient not found");

        var gp = await _context.Users.FindAsync(dto.GeneralPractitionerId);
        if (gp == null || gp.RoleId != 2) // Ensure it's a GP
            return NotFound("General Practitioner not found");

        patient.GeneralPractitionerId = gp.Id;

        await _context.SaveChangesAsync();

        return Ok(new { message = "General Practitioner assigned successfully" });
    }
}