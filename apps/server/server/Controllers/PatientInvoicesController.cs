using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Extensions;
using ZhmApi.Services;

namespace ZhmApi.Controllers
{
    [Authorize(Roles = "Patient")]
    [ApiController]
    [Route("api/patient/invoices")]
    public class PatientInvoicesController : ControllerBase
    {
        private readonly PatientInvoiceService _service;

        public PatientInvoicesController(PatientInvoiceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyInvoices()
        {
            var patientId = User.GetUserId();
            return Ok(await _service.GetForPatient(patientId));
        }
    }
}
