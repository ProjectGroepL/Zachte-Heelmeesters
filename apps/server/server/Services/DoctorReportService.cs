using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Dtos;
using ZhmApi.Models;

namespace ZhmApi.Services
{
    public class DoctorReportService
    {
        private readonly ApiContext _context;

        public DoctorReportService(ApiContext context)
        {
            _context = context;
        }

        public async Task<List<PatientReportDto>> GetForDoctor(int doctorId)
        {
            return await _context.AppointmentReports
                .Include(r => r.Items)
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.Referral)
                .Where(r =>
                    r.Appointment.Referral.DoctorId == doctorId &&
                    _context.AccesssRequests.Any(ar =>
                        ar.AppointmentId == r.AppointmentId &&
                        ar.SpecialistId == doctorId &&
                        ar.Status == AccessRequestStatus.Approved
                    )
                )
                .Select(r => new PatientReportDto
                {
                    Id = r.Id,
                    Summary = r.Summary,
                    CreatedAt = r.CreatedAt,
                    Items = r.Items.Select(i => i.Description).ToList()
                })
                .ToListAsync();
        }
    }

}
