using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Extensions;
using ZhmApi.Models;
using ZhmApi.Services;

namespace ZhmApi.Controllers
{
[Authorize(Roles = "Huisarts")]
[ApiController]
[Route("api/doctor/appointments")]
public class DoctorAppointmentsController : ControllerBase
{
    private readonly ApiContext _context;

    public DoctorAppointmentsController(ApiContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var doctorId = User.GetUserId();

        var appointments = await _context.Appointments
            .Include(a => a.Referral)
                .ThenInclude(r => r.Patient)
            .Select(a => new
            {
                id = a.Id,
                date = a.Date,
                patientName = a.Referral.Patient.FirstName + " " + a.Referral.Patient.LastName,

                requestStatus = _context.AccesssRequests
                    .Where(ar =>
                        ar.AppointmentId == a.Id &&
                        ar.SpecialistId == doctorId
                    )
                    .OrderByDescending(ar => ar.RequestedAt)
                    .Select(ar => ar.Status)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(appointments);
    }
}
}