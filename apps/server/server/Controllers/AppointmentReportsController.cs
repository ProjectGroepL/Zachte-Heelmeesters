using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Dtos;
using ZhmApi.Extensions;
using ZhmApi.Services;

[Authorize(Roles = "Specialist,Patient")]
[ApiController]
[Route("api/specialist/appointments/{appointmentId}/report")]
public class AppointmentReportsController : ControllerBase
{
    private readonly AppointmentReportService _service;

    public AppointmentReportsController(AppointmentReportService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentReportDto>> Create(
        int appointmentId,
        [FromBody] CreateAppointmentReportDto dto
    )
    {
        var specialistId = User.GetUserId();

        var report = await _service.CreateReport(
            specialistId,
            appointmentId,
            dto
        );

        return Ok(new AppointmentReportDto
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
        });
    }

    [HttpGet]
    public async Task<ActionResult<AppointmentReportDto>> Get(int appointmentId)
    {
        var report = await _service.GetByAppointment(appointmentId);
        if (report == null) return NotFound();

        return Ok(new AppointmentReportDto
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
        });
    }
    
}
