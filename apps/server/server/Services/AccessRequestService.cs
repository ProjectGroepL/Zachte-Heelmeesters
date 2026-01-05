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

        public async Task<AccessRequest> RequestAccess
        (
            int specialistId,
            int patientId,
            string reason
        )
        {
            var exists = await _context.AccesssRequests.AnyAsync(r =>
                r.SpecialistId == specialistId &&
                r.PatientId == patientId &&
                r.Status == AccessRequestStatus.Pending
            );

            if (exists)
                throw new InvalidOperationException("Er bestaat al een open aanvraag voor deze patiënt.");
                
            var request = new AccessRequest
            {
                SpecialistId = specialistId,
                PatientId = patientId,
                Reason = reason,
                Status = AccessRequestStatus.Pending
            };

            _context.AccesssRequests.Add(request);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task RevokeAccess(int requestId, int patientId)
        {
            var request = await _context.AccesssRequests
            .FirstOrDefaultAsync(r => 
            r.Id == requestId &&
            r.PatientId == patientId &&
            r.Status == AccessRequestStatus.Approved);
            
            if (request == null)
                throw new InvalidOperationException("No active premission"); 
            
            request.Status = AccessRequestStatus.Revoked; 
            await _context.SaveChangesAsync();
        }

        public async Task DecideRequest(
            int requestId,
            int patientId,
            bool approved
        )
        {
            var request = await _context.AccesssRequests
            .FirstOrDefaultAsync(r => 
            r.Id == requestId &&
            r.PatientId == patientId &&
            r.Status == AccessRequestStatus.Pending);

            if (request == null)
                throw new InvalidOperationException("No pending request found");

            request.Status = approved
            ? AccessRequestStatus.Approved
            : AccessRequestStatus.Denied;

            request.DecidedAt = DateTime.UtcNow; 

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
                .Where(r => r.PatientId == patientId && r.Status == AccessRequestStatus.Pending)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();
        }
    }
}