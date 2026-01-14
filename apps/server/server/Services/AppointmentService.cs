using ZhmApi.Data;
using ZhmApi.Dtos;
using ZhmApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace ZhmApi.Services
{
    public class AppointmentService
    {
        private readonly ApiContext _context;

        public AppointmentService(ApiContext context)
        {
            _context = context;
        }

        // Get a single appointment by ID
        public async Task<AppointmentDto?> GetAppointmentAsync(int id)
        {
            return await _context.Appointments
                .Where(a => a.Id == id)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    ReferralId = a.ReferralId,
                    Notes = a.Notes,
                    Status = a.Status,
                    TreatmentDescription = a.TreatmentDescription,
                    TreatmentInstructions = a.TreatmentInstructions,
                    PatientName = a.Patient.FirstName + " " + a.Patient.LastName,
                    Date = a.Date
                })
                .FirstOrDefaultAsync();
        }
    }
}