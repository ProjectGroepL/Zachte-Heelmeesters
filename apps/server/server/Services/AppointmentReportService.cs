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
                .FirstOrDefaultAsync(a =>
                    a.Id == appointmentId &&
                    a.SpecialistId == specialistId &&
                    a.Status == AppointmentStatus.Scheduled
                );

            if (appointment == null)
                throw new InvalidOperationException("Appointment not found or not allowed");

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
    }
}
