

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Dtos;
using ZhmApi.Extensions;
using ZhmApi.Services;
using ZhmAPi.Dtos;
using ZhmApi.Dtos; // Ensure the correct namespace for SetCoveredAmountDto is included

[ApiController]
[Route("api/insurance/invoices")]
[Authorize(Roles = "Administratie,Zorgverzekeraar")]
public class insuranceInvoiceController : ControllerBase
{
    private readonly InsuranceInvoiceService _service;
    
    public insuranceInvoiceController(InsuranceInvoiceService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateInsuranceInvoiceDto dto)
    {
        try
        {
            var adminId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var invoice = await _service.CreateInvoice(
                adminId,
                dto.AppointmentReportId,
                dto.InsurerId
            );

            return Ok(new InsuranceInvoiceDto
            {
                InvoiceId = invoice.Id,
                Amount = invoice.Amount,
                Date = invoice.CreatedAt
            });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [Authorize(Roles = "Administratie")]
    [HttpGet("insurers")]
    public async Task<IActionResult> GetInsurers()
    {
        return Ok(await _service.GetInsurers());
    }

    [Authorize(Roles = "Zorgverzekeraar")]
    [HttpPost("{id}/coverage")]
    public async Task<IActionResult> SetCoverage(
        int id,
        SetCoveredAmountDto dto
    )
    {
        var insurerId = User.GetUserId();
        await _service.SetCoveredAmount(id, insurerId, dto.CoveredAmount);
        return NoContent();
    }

    [Authorize(Roles = "Zorgverzekeraar")]
[HttpGet]
public async Task<IActionResult> GetForInsurer()
{
    var insurerId = User.GetUserId();
    return Ok(await _service.GetInvoicesForInsurer(insurerId));
}
}
