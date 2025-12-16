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