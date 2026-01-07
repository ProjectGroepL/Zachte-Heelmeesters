using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Dtos;
using ZhmApi.Extensions;
using ZhmApi.Services;

[Authorize(Roles = "Specialist,Huisarts")]
[ApiController]
[Route("api/specialist/patients")]
public class SpecialistMedicalDocumentsController : ControllerBase
{
    private readonly MedicalDocumentService _service;

    public SpecialistMedicalDocumentsController(MedicalDocumentService service)
    {
        _service = service;
    }

    [HttpGet("{patientId}/medical-documents")]
    public async Task<ActionResult<IEnumerable<MedicalDocumentDto>>> GetPatientDocs(
        int patientId
    )
    {
        var specialistId = User.GetUserId();
        var docs = await _service.GetForSpecialist(specialistId, patientId);

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
}
