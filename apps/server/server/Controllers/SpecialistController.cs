using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZhmApi.Dtos;
using ZhmApi.Services;
using ZhmApi.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using ZhmApi.Data;
using Microsoft.EntityFrameworkCore;

namespace ZhmApi.Controllers
{
    [Authorize(Roles = "Specialist")]
    [ApiController]
    [Route("api/specialist/access-requests")]
    public class SpecialistAccessController : ControllerBase
    {
        private readonly AccessRequestService _service;
        private readonly ApiContext _context;

        public SpecialistAccessController(
            AccessRequestService service,
            ApiContext context
        )
        {
            _service = service;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> RequestAccess(
            [FromBody] CreateAccessRequestDto dto
        )
        {
            var specialistId = User.GetUserId();

            try
            {
                var request = await _service.RequestAccess(
                    specialistId,
                    dto.AppointmentId,
                    dto.Reason
                );

                return Ok(new AccessRequestDto
                {
                    Id = request.Id,
                    SpecialistId = request.SpecialistId,
                    PatientId = request.PatientId,
                    Status = request.Status,
                    RequestedAt = request.RequestedAt
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<AccessRequestDto>>> GetMyRequests()
        {
            var specialistId = User.GetUserId();
            var requests = await _service.GetRequestsForSpecialist(specialistId);
            return Ok(requests);
        }

        
    }

}