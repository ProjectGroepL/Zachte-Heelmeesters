using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Models;

namespace ZhmApi.Services
{
    public class MedicalDocumentService
    {
        private readonly ApiContext _context;

        public MedicalDocumentService(ApiContext context)
        {
            _context = context;
        }

        // PATIENT: all own documents
        public async Task<List<MedicalDocument>> GetForPatient(int patientId)
        {
            return await _context.MedicalDocuments
                .Where(d => d.PatientId == patientId)
                .OrderByDescending(d => d.createdAt)
                .ToListAsync();
        }

        // PATIENT: update status
        public async Task UpdateStatus(
            int documentId,
            int patientId,
            MedicalDocumentStatus status
        )
        {
            var doc = await _context.MedicalDocuments
                .FirstOrDefaultAsync(d =>
                    d.Id == documentId &&
                    d.PatientId == patientId
                );

            if (doc == null)
                throw new InvalidOperationException("Document not found");

            doc.Status = status;
            await _context.SaveChangesAsync();
        }

        // SPECIALIST: only FINAL docs if access approved
        public async Task<List<MedicalDocument>> GetForSpecialist(
            int specialistId,
            int patientId
        )
        {
            var hasAccess = await _context.AccesssRequests.AnyAsync(r =>
                r.SpecialistId == specialistId &&
                r.PatientId == patientId &&
                r.Status == AccessRequestStatus.Approved
            );

            if (!hasAccess)
                throw new UnauthorizedAccessException();

            return await _context.MedicalDocuments
                .Where(d =>
                    d.PatientId == patientId &&
                    d.Status == MedicalDocumentStatus.Final
                )
                .OrderByDescending(d => d.createdAt)
                .ToListAsync();
        }
    }
}
