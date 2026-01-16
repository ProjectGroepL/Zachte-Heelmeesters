using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Models;
using Microsoft.AspNetCore.Identity;
using ZhmApi.Dtos;

namespace ZhmApi.Services
{
    public class InsuranceInvoiceService
    {
        private readonly ApiContext _context;
        private readonly NotificationService _notificationService;
        private readonly UserManager<User> _userManager;

        public InsuranceInvoiceService(
            ApiContext context,
            NotificationService notificationService,
            UserManager<User> userManager
        )
        {
            _context = context;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<InsuranceInvoice> CreateInvoice(
            int adminId,
            int reportId,
            int insurerId
        )
        {
            var report = await _context.AppointmentReports
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
                throw new InvalidOperationException("Report not found");
            
            if (report.Status != AppointmentReportStatus.SentToAdmin)
                throw new InvalidOperationException("Report already processed");

            var insurer = await _userManager.FindByIdAsync(insurerId.ToString());
            if (insurer == null)
                throw new InvalidOperationException("Insurer not found");

            var isInsurer = await _userManager.IsInRoleAsync(
                insurer,
                "Zorgverzekeraar"
            );

            if (!isInsurer)
                throw new InvalidOperationException("User is not a zorgverzekeraar");

            var invoice = new InsuranceInvoice
            {
                AppointmentReportId = reportId,
                InsurerId = insurerId,
                Amount = report.TotalCost,
                CoveredAmount = null // insurer will fill this later
            };

            _context.InsuranceInvoices.Add(invoice);
            
            report.Status = AppointmentReportStatus.Invoiced;

            await _context.SaveChangesAsync();

            // 🔔 notify insurer
            await _notificationService.Create(
                insurerId,
                $"Nieuwe declaratie ontvangen (factuur #{invoice.Id})",
                NotificationType.General
            );

            return invoice;
        }

        public async Task<List<UserSummaryDto>> GetInsurers()
        {
            var insurers = await _userManager.GetUsersInRoleAsync("Zorgverzekeraar");

            return insurers.Select(u => new UserSummaryDto
            {
                Id = u.Id,
                DisplayName = $"{u.FirstName} {u.LastName}"
            }).ToList();
        }

        public async Task SetCoveredAmount(
            int invoiceId,
            int insurerId,
            decimal coveredAmount
        )
        {
            var invoice = await _context.InsuranceInvoices
                .Include(i => i.AppointmentReport)
                    .ThenInclude(r => r.Appointment)
                        .ThenInclude(a => a.Referral)
                .FirstOrDefaultAsync(i =>
                    i.Id == invoiceId &&
                    i.InsurerId == insurerId
                );

            if (invoice == null)
                throw new InvalidOperationException("Invoice not found");

            if (coveredAmount < 0 || coveredAmount > invoice.Amount)
                throw new InvalidOperationException("Invalid covered amount");

            invoice.CoveredAmount = coveredAmount;

            await _context.SaveChangesAsync();

            var patientId =
                invoice.AppointmentReport
                    .Appointment
                    .Referral
                    .PatientId;

            await _notificationService.Create(
                patientId,
                $"Uw declaratie is verwerkt (factuur #{invoice.Id})",
                NotificationType.General
            );
        }

        public async Task<List<InsuranceInvoiceForInsurerDto>> GetInvoicesForInsurer(int insurerId)
{
    return await _context.InsuranceInvoices
        .Include(i => i.AppointmentReport)
        .Where(i => i.InsurerId == insurerId)
        .Select(i => new InsuranceInvoiceForInsurerDto
        {
            InvoiceId = i.Id,
            Date = i.CreatedAt,
            Amount = i.Amount,
            CoveredAmount = i.CoveredAmount,
            PatientPays = i.CoveredAmount == null
                ? i.Amount
                : i.Amount - i.CoveredAmount.Value
        })
        .OrderByDescending(i => i.Date)
        .ToListAsync();
}
    }
    
}