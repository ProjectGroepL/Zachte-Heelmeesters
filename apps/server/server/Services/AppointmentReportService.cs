using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Dtos;
using ZhmApi.Models;

namespace ZhmApi.Services
{
    public class AppointmentReportService
    {
        private readonly ApiContext _context;

        public AppointmentReportService(ApiContext context)
        {
            _context = context;
        }

        public async Task<AppointmentReport> CreateReport(
            int specialistId,
            int appointmentId,
            CreateAppointmentReportDto dto
        )
        {
            var appointment = await _context.Appointments
                .Include(a => a.Referral)
                    .ThenInclude(r => r.Patient)
                .FirstOrDefaultAsync(a =>
                    a.Id == appointmentId &&
                    a.SpecialistId == specialistId &&
                    a.Status == AppointmentStatus.Scheduled
                );

            if (appointment == null)
                throw new InvalidOperationException("Appointment not found");

            var report = new AppointmentReport
            {
                AppointmentId = appointmentId,
                Summary = dto.Summary,
                Items = dto.Items.Select(i => new AppointmentReportItem
                {
                    Description = i.Description,
                    Cost = i.Cost
                }).ToList()
            };

            report.TotalCost = report.Items.Sum(i => i.Cost);

            appointment.Status = AppointmentStatus.Completed;

            // 👇 BELANGRIJK
            report.Status = AppointmentReportStatus.Created;

            _context.AppointmentReports.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }

        public async Task<AppointmentReport?> GetByAppointment(int appointmentId)
        {
            return await _context.AppointmentReports
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.AppointmentId == appointmentId);
        }

        public async Task<AppointmentReportInternalDto> GetInternal(int reportId)
        {
            var report = await _context.AppointmentReports 
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
                throw new InvalidOperationException("Report not found");

            return new AppointmentReportInternalDto
            {
                Id = report.Id,
                Summary = report.Summary,
                CreatedAt = report.CreatedAt,
                TotalCost = report.TotalCost,
                Items = report.Items.Select(i => new AppointmentReportItemDto
                {
                    Description = i.Description,
                    Cost = i.Cost
                }).ToList()
            };
        }

        public async Task<AppointmentReportPatientDto?> GetForPatientByAppointment(
            int appointmentId,
            int patientId
        )
        {
            var report = await _context.AppointmentReports 
                .Include(r => r.Items)
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.Referral)
                .FirstOrDefaultAsync(r => 
                r.AppointmentId == appointmentId 
                && r.Appointment.Referral.PatientId == patientId);

            if (report == null)
                return null; 

            return new AppointmentReportPatientDto
            {
                Id = report.Id,
                Summary = report.Summary,
                CreatedAt = report.CreatedAt,
                Items = report.Items.Select(i => i.Description).ToList()
            };
        }

        public async Task SendToAdmin(int reportId, int specialistId)
        {
            var report = await _context.AppointmentReports
                .Include(r => r.Appointment)
                .FirstOrDefaultAsync(r =>
                    r.Id == reportId &&
                    r.Appointment.SpecialistId == specialistId
                );

            if (report == null)
                throw new InvalidOperationException("Report not found");

            report.Status = AppointmentReportStatus.SentToAdmin;
            await _context.SaveChangesAsync();
        }
    }
}
