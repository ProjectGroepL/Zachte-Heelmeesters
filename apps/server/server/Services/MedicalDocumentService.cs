using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Dtos;
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
            int appointmentId
        )
        {
            var request = await _context.AccesssRequests
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.Referral)
                .FirstOrDefaultAsync(r =>
                    r.SpecialistId == specialistId &&
                    r.AppointmentId == appointmentId &&
                    r.Status == AccessRequestStatus.Approved
                );

            if (request == null)
                return new List<MedicalDocument>();

            var patientId = request.Appointment.Referral.PatientId;

            return await _context.MedicalDocuments
                .Where(d =>
                    d.PatientId == patientId &&
                    d.Status == MedicalDocumentStatus.Final
                )
                .OrderByDescending(d => d.createdAt)
                .ToListAsync();
        }

        public async Task<MedicalDocument> Create(
            int doctorId,
            CreateMedicalDocumentDto dto
        )
        {
            var hasRelation = await _context.DoctorPatients.AnyAsync(dp =>
                dp.DoctorId == doctorId &&
                dp.PatientId == dto.PatientId
            );

            if (!hasRelation)
                throw new UnauthorizedAccessException("No doctor-patient relation");

            var doc = new MedicalDocument
            {
                PatientId = dto.PatientId,
                AppointmentId = dto.AppointmentId,
                Title = dto.Title,
                Content = dto.Content,
                Status = MedicalDocumentStatus.Draft,
                CreatedBy = doctorId.ToString(),
                createdAt = DateTime.UtcNow
            };

            _context.MedicalDocuments.Add(doc);
            await _context.SaveChangesAsync();

            return doc;
        }

        public async Task<List<MedicalDocument>> GetForDoctor(int doctorId)
        {
            return await _context.MedicalDocuments
                .Where(d => d.CreatedBy == doctorId.ToString())
                .OrderByDescending(d => d.createdAt)
                .ToListAsync();
        }

        // DOCTOR: update status
        public async Task UpdateStatusByDoctor(
            int documentId,
            int doctorId,
            MedicalDocumentStatus status
        )
        {
            var doc = await _context.MedicalDocuments
                .FirstOrDefaultAsync(d =>
                    d.Id == documentId &&
                    d.CreatedBy == doctorId.ToString()
                );

            if (doc == null)
                throw new UnauthorizedAccessException();

            doc.Status = status;
            await _context.SaveChangesAsync();
        }

       public async Task<List<MedicalDocument>> GetForSpecialistByAppointment(
            int specialistId,
            int appointmentId
        )
        {
            var request = await _context.AccesssRequests
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.Referral)
                .FirstOrDefaultAsync(r =>
                    r.SpecialistId == specialistId &&
                    r.AppointmentId == appointmentId &&
                    r.Status == AccessRequestStatus.Approved
                );

            if (request == null)
                return new List<MedicalDocument>();

            var patientId = request.Appointment.Referral.PatientId;

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
