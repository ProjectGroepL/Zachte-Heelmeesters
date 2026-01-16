using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Dtos;

namespace ZhmApi.Services
{
    public class PatientInvoiceService
    {
        private readonly ApiContext _context;

        public PatientInvoiceService(ApiContext context)
        {
            _context = context;
        }

        public async Task<List<PatientInvoiceViewDto>> GetForPatient(int patientId)
        {
            return await _context.InsuranceInvoices
                .Include(i => i.AppointmentReport)
                    .ThenInclude(r => r.Appointment)
                        .ThenInclude(a => a.Referral)
                .Where(i =>
                    i.AppointmentReport
                        .Appointment
                        .Referral
                        .PatientId == patientId
                )
                .Select(i => new PatientInvoiceViewDto
                {
                    InvoiceId = i.Id,
                    PatientPays = i.Amount - (i.CoveredAmount ?? 0),
                    Date = i.CreatedAt
                })
                .ToListAsync();
        }
    }
}
