using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZhmApi.Dtos;
using ZhmApi.Services;
using ZhmApi.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ZhmApi.Controllers
{
    [Authorize(Roles = "Specialist")]
    [ApiController]
    [Route("api/specialist/access-request")]
    public class SpecialistAccessController : ControllerBase
    {
        private readonly AccessRequestService _service; 

        public SpecialistAccessController(AccessRequestService service)
        {
            _service = service;
        }
        
        [HttpPost]
        public async Task<ActionResult<AccessRequestDto>> RequestAccesss(
            [FromBody] CreateAccessRequestDto dto
        )
        {
            try{
            var specialistId = User.GetUserId();

            var request = await _service.RequestAccess(
                specialistId,
                dto.PatientId,
                dto.Reason
            );

            return Ok(new AccessRequestDto
            {
                Id = request.Id,
                SpecialistId = request.SpecialistId,
                Status = request.Status,
                RequestedAt = request.RequestedAt
            });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        
    }
}