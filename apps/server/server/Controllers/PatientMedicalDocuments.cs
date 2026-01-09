using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Dtos;
using ZhmApi.Extensions;
using ZhmApi.Services;

[Authorize(Roles = "Patient,Huisarts")]
[ApiController]
[Route("api/patient/medical-documents")]
public class PatientMedicalDocumentsController : ControllerBase
{
    private readonly MedicalDocumentService _service;


    public PatientMedicalDocumentsController(MedicalDocumentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicalDocumentDto>>> GetMine()
    {
        var patientId = User.GetUserId();
        var docs = await _service.GetForPatient(patientId);

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
        try
        {
            var patientId = User.GetUserId();
            await _service.UpdateStatus(id, patientId, dto.Status);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    
}
