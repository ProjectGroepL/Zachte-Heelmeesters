using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Dtos;
using ZhmApi.Extensions;
using ZhmApi.Services;

namespace ZhmApi.Controllers
{
    [Authorize(Roles = "Patient")]
    [ApiController]
    [Route("api/patient/access-requests")]
    public class PatientAccessController : ControllerBase
    {
        private readonly AccessRequestService _service;

        public PatientAccessController(AccessRequestService service)
        {
            _service = service;
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<AccessRequestDto>>> GetMyRequests()
        {
            // 1. Haal de ID van de ingelogde PATIENT op
            var patientId = User.GetUserId();

            // 2. BELANGRIJK: Gebruik GetRequestsForPatient (niet Specialist!)
            var requests = await _service.GetRequestsForPatient(patientId);

            // 3. Map naar DTO
            var dtos = requests.Select(r => new AccessRequestDto
            {
                Id = r.Id,
                SpecialistId = r.SpecialistId,
                SpecialistName = r.Specialist?.UserName ?? "Onbekende Specialist", 
                PatientId = r.PatientId,
                Reason = r.Reason,
                Status = r.Status, // Dit is de Enum
                RequestedAt = r.RequestedAt
            });

            return Ok(dtos);
        }
        [HttpPost("{id}/decision")]
        public async Task<IActionResult> Decide(
            int id,
            [FromBody] DecideAccessRequesetDto dto
        )
        {
            try
            {
                var patientId = User.GetUserId();

                await _service.DecideRequest(
                    id,
                    patientId,
                    dto.Approved
                );

                return NoContent();
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}