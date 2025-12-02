using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Dtos;
using ZhmApi.Models;
using ZhmApi.Services;

namespace ZhmApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReferralsController : ControllerBase
    {
        private IReferralService _referralService;

        [HttpPost]
        public async Task<IActionResult> CreateReferral(CreateReferralRequest request)
        {
            var doctorId = int.Parse(User.FindFirst("id")!.Value);

            var result = await _referralService.CreateReferralAsync(doctorId, request);

            if(!result.Succes)
            {
                if(result.Error == "Patiënt niet toegestaan") return Forbid(result.Error);
                if (result.Error == "Deze doorverwijzing bestaat al") return Conflict(result.Error);
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetReferral), new { id = result.Referral!.Id}, result.Referral);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReferral(int id)
        {
            var referral = await _referralService.GetReferralAsync(id);
            return referral == null? NotFound() : Ok(referral);
        }
    }
}