using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using ZhmApi.Data;
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
            var request = new AccessRequest
            {
                SpecialistId = specialistId,
                PatientId = patientId,
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
    }
}