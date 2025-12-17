using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Models;

namespace ZhmApi.Controllers
{
    [Authorize(Roles = "Specialist")]
    [ApiController]
    [Route("api/[controller]")]
    public class SpecialistPrivateAgendaController : ControllerBase
    {
        private readonly ApiContext _context;

        public SpecialistPrivateAgendaController(ApiContext context)
        {
            _context = context;
        }
        
        [HttpGet("{specialistId:int}")]
        [Authorize(Roles = "Admin, Specialist")]
        public async Task<ActionResult<IEnumerable<SpecialistPrivateAgenda>>> GetBySpecialistId(int specialistId)
        {
            var items = await _context.SpecialistPrivateAgendas
                .Where(a => a.UserId == specialistId)
                .ToListAsync();

            return Ok(items);
        }
    }
}