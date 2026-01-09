using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Dtos;
using ZhmApi.Extensions;
using ZhmApi.Services;

[Authorize(Roles = "Huisarts")]
[ApiController]
[Route("api/doctor/medical-documents")]
public class DoctorMedicalDocumentsController : ControllerBase
{
    private readonly MedicalDocumentService _service;

    public DoctorMedicalDocumentsController(MedicalDocumentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicalDocumentDto>>> GetMine()
    {
        var doctorId = User.GetUserId();
        var docs = await _service.GetForDoctor(doctorId);

        return Ok(docs.Select(d => new MedicalDocumentDto
        {
            Id = d.Id,
            Title = d.Title,
            Content = d.Content,
            Status = d.Status,
            CreatedAt = d.createdAt,
            CreatedBy = d.CreatedBy
        }));
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateMedicalDocumentStatusDto dto
    )
    {
        var doctorId = User.GetUserId();
        await _service.UpdateStatusByDoctor(id, doctorId, dto.Status);
        return NoContent();
    }

    [HttpPost("")]
    [Consumes("application/json")]
    public async Task<IActionResult> Create(
        [FromBody] CreateMedicalDocumentDto dto
    )
    {
         try
        {
            var doctorId = User.GetUserId();
            var doc = await _service.Create(doctorId, dto);
            return Ok(doc);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
