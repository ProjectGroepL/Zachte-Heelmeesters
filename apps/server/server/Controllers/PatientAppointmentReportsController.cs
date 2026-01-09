using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Dtos;
using ZhmApi.Services;

[Authorize(Roles = "Patient")]
[ApiController]
[Route("api/patient/appointments/{appointmentId}/report")]
public class PatientAppointmentReportsController : ControllerBase
{
    private readonly AppointmentReportService _service;

    public PatientAppointmentReportsController(AppointmentReportService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<AppointmentReportDto>> Get(int appointmentId)
    {
        var report = await _service.GetByAppointment(appointmentId);
        if (report == null) return NotFound();

        return new AppointmentReportDto
        {
            Id = report.Id,
            Summary = report.Summary,
            TotalCost = report.TotalCost,
            CreatedAt = report.CreatedAt,
            Items = report.Items.Select(i => new AppointmentReportItemDto
            {
                Description = i.Description,
                Cost = i.Cost
            }).ToList()
        };
    }
}
