using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Dtos;
using ZhmApi.Models;

namespace ZhmApi.Services
{
    public class AdminReportService
    {
        private readonly ApiContext _context;

        public AdminReportService(ApiContext context)
        {
            _context = context;
        }

        public async Task<List<AdminReportOverviewDto>> GetPendingReports()
        {
            return await _context.AppointmentReports
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.Referral)
                        .ThenInclude(r => r.Patient)
                .Where(r => r.Status == AppointmentReportStatus.SentToAdmin)
                .Select(r => new AdminReportOverviewDto
                {
                    ReportId = r.Id,
                    PatientName =
                        r.Appointment.Referral.Patient.FirstName + " " +
                        r.Appointment.Referral.Patient.LastName,
                    CreatedAt = r.CreatedAt,
                    TotalCost = r.TotalCost
                })
                .ToListAsync();
        }
    }

}