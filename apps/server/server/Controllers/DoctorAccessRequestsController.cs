using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Dtos;
using ZhmApi.Extensions;
using ZhmApi.Services;

namespace ZhmApi.Controllers
{
    
    [Authorize(Roles = "Huisarts")]
[ApiController]
[Route("api/doctor/access-requests")]
public class DoctorAccessRequestsController : ControllerBase
{
    private readonly AccessRequestService _service;

    public DoctorAccessRequestsController(AccessRequestService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Request(CreateAccessRequestDto dto)
    {
        var doctorId = User.GetUserId();
        await _service.RequestDoctorAccess(
            doctorId,
            dto.AppointmentId,
            dto.Reason
        );
        return NoContent();
    }
}

}