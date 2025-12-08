using System.Data;
using ZhmApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZhmApi.Dtos;
using ZhmApi.Models;
using ZhmApi.Services;
using System.Security.Claims;

namespace ZhmApi.Controllers
{
    [Authorize(Roles = "Huisarts,Specialist,Patient")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReferralsController : ControllerBase
    {
        private readonly IReferralService _referralService;
        private readonly ApiContext _db;

        public ReferralsController(IReferralService referralService, ApiContext db)
        {
            _referralService = referralService;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReferral(CreateReferralRequest request)
        {
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

            if (!int.TryParse(idClaim, out var doctorId))
            {
                return Unauthorized("Invalid token claims");
            }

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

        // GET /api/referrals -> returns referrals for current authenticated doctor
        [HttpGet]
        public async Task<IActionResult> GetForCurrentDoctor()
        {
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

            if (!int.TryParse(idClaim, out var doctorId))
            {
                return Unauthorized("Invalid token claims");
            }

            var list = await _referralService.GetDoctorReferralsAsync(doctorId);
            return Ok(list);
        }

        // DEBUG: returns raw DoctorPatients and Referrals for the authenticated doctor
        [HttpGet("debug")]
        public async Task<IActionResult> DebugForCurrentDoctor()
        {
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

            if (!int.TryParse(idClaim, out var doctorId))
            {
                return Unauthorized("Invalid token claims");
            }

            var doctorPatients = await _db.DoctorPatients
                .Where(dp => dp.DoctorId == doctorId)
                .Include(dp => dp.Patient)
                .ToListAsync();

            var referrals = await _db.Referrals
                .Where(r => r.DoctorId == doctorId)
                .Include(r => r.Patient)
                .Include(r => r.Treatment)
                .ToListAsync();

            return Ok(new { doctorPatients, referrals });
        }

        // GET: api/referrals/{patientID}
        [HttpGet("patient")]
        public async Task<ActionResult<IEnumerable<ReferralDto>>> GetReferrals()
        {
            // Haal userId uit JWT claim van ingelogde gebruiker
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Niet ingelogd");

            // Selecteer de referrals van deze gebruiker
            var referrals = await _db.Referrals
                .Include(r => r.Treatment)
                .Where(r => r.PatientId == userId && r.Status == "open" && r.ValidUntil >= DateTime.UtcNow)
                .Select(r => new ReferralDto
                {
                    Id = r.Id,
                    TreatmentDescription = r.Treatment.Description,
                    Instructions = r.Treatment.Instructions,
                    Status = r.Status,
                    Notes = r.Notes,
                    CreatedAt = r.CreatedAt,
                    ValidUntil = r.ValidUntil
                })
                .ToListAsync();

            return Ok(referrals);
        }
    }
}