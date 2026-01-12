using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using ZhmApi.Data;
using ZhmApi.Dtos;
using ZhmApi.Models;

namespace ZhmApi.Services
{
    public class AccessRequestService {
        private readonly ApiContext _context; 

        public AccessRequestService(ApiContext context)
        {
            _context = context;
        } 

        public async Task<AccessRequest> RequestAccess(int specialistId, int appointmentId, string reason)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Referral)
                .FirstOrDefaultAsync(a =>
                    a.Id == appointmentId &&
                    a.SpecialistId == specialistId
                );

            if (appointment == null)
                throw new InvalidOperationException("Appointment not found");

            var exists = await _context.AccesssRequests.AnyAsync(r =>
                r.AppointmentId == appointmentId &&
                r.Status == AccessRequestStatus.Pending
            );


            if (exists)
                throw new InvalidOperationException("Access request already exists");

            var request = new AccessRequest
            {
                AppointmentId = appointmentId,
                SpecialistId = specialistId,
                PatientId = appointment.Referral.PatientId,
                TreatmentId = appointment.Referral.TreatmentId,
                Status = AccessRequestStatus.Pending,
                Reason = reason,
                RequestedAt = DateTime.UtcNow
            };

             _context.AccesssRequests.Add(request);
            await _context.SaveChangesAsync(); // request.Id NOW EXISTS

            _context.Notifications.Add(new Notification
            {
                UserId = request.PatientId,
                Type = NotificationType.AccessRequest,
                Message = "A specialist has requested access to your medical file.",
                AccessRequestId = request.Id
            });

            await _context.SaveChangesAsync();

            return request;
        }

        public async Task RevokeAccess(int requestId, int patientId)
        {
            var request = await _context.AccesssRequests
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.Referral) // 🔥 ESSENTIEEL
                .FirstOrDefaultAsync(r =>
                    r.Id == requestId &&
                    r.PatientId == patientId &&
                    r.Status == AccessRequestStatus.Approved
                );

            if (request == null)
                throw new InvalidOperationException("No active permission");

            // 1️⃣ revoke request
            request.Status = AccessRequestStatus.Revoked;

            // 2️⃣ cancel appointment
            request.Appointment.Status = AppointmentStatus.Cancelled;

            // 3️⃣ restore referral
            request.Appointment.Referral.Status = ReferralStatus.Open;

            await _context.SaveChangesAsync();
        }

        public async Task DecideRequest(int requestId, int patientId, bool approved)
        {
            var request = await _context.AccesssRequests
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.Referral)
                .FirstOrDefaultAsync(r =>
                    r.Id == requestId &&
                    r.PatientId == patientId &&
                    r.Status == AccessRequestStatus.Pending
                );

            if (request == null)
                throw new InvalidOperationException("No pending request found");

            request.Status = approved
                ? AccessRequestStatus.Approved
                : AccessRequestStatus.Denied;

            request.DecidedAt = DateTime.UtcNow;

            request.Appointment.Status = approved
                ? AppointmentStatus.Scheduled
                : AppointmentStatus.Cancelled;

            // 🔔 notify SPECIALIST
            _context.Notifications.Add(new Notification
            {
                UserId = request.SpecialistId,
                Type = NotificationType.AccessRequestDecision,
                Message = approved
                    ? "Your access request was approved."
                    : "Your access request was denied.",
                AccessRequestId = request.Id
            });

            await _context.SaveChangesAsync();
        }

        public async Task<List<AccessRequestDto>> GetRequestsForSpecialist(int specialistId)
        {
            return await _context.AccesssRequests
                .Where(r => r.SpecialistId == specialistId)
                .Include(r => r.Patient)
                .Include(r => r.Specialist)
                .OrderByDescending(r => r.RequestedAt)
                .Select(r => new AccessRequestDto
                {
                    Id = r.Id,
                    SpecialistId = r.SpecialistId,
                    SpecialistName = r.Specialist.FirstName + " " + r.Specialist.LastName,
                    PatientId = r.PatientId,
                    PatientName = r.Patient.FirstName + " " + r.Patient.LastName,
                    Reason = r.Reason,
                    Status = r.Status,
                    RequestedAt = r.RequestedAt
                })
                .ToListAsync();
        }

        public async Task<List<AccessRequest>> GetRequestsForPatient(int patientId)
        {
            return await _context.AccesssRequests
                .Include(r => r.Specialist)
                .Where(r =>
                    r.PatientId == patientId &&
                    (r.Status == AccessRequestStatus.Pending ||
                    r.Status == AccessRequestStatus.Approved)
                )
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();
        }
    }
}