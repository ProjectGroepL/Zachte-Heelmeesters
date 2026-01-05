using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Dtos;
using ZhmApi.Models;

namespace ZhmApi.Services
{
    public class ReferralService : IReferralService
    {
        private readonly ApiContext _db;

        public ReferralService(ApiContext db)
        {
            _db = db;
        }

        public async Task<(bool Succes, string? Error, Referral? Referral)> CreateReferralAsync(
            int doctorId, CreateReferralRequest request)
        {
            bool allowed = await _db.DoctorPatients
                .AnyAsync(dp => dp.DoctorId == doctorId && dp.PatientId == request.PatientId);
            if(!allowed)
                return(false, "Patient niet toegestaan", null);

            bool exists = await _db.Referrals
                .AnyAsync(r => r.PatientId == request.PatientId && r.TreatmentId == request.TreatmentId);

            if(exists)
                return(false, "deze doorverwijzing bestaat al", null);   

            var referral = new Referral
            {
                DoctorId = doctorId,
                PatientId = request.PatientId,
                TreatmentId = request.TreatmentId,
                Notes = request.Notes,
                Status = string.IsNullOrWhiteSpace(request.Status) ? "open" : request.Status!
            };

            _db.Referrals.Add(referral);
            await _db.SaveChangesAsync();

            return (true,null,referral);
        }
        public async Task<Referral?> GetReferralAsync(int id)
        {
            return await _db.Referrals
                    .Include(r => r.Treatment)
                    .Include(r => r.Patient)
                    .FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<IEnumerable<ReferralResponse>>  GetPatientReferralAsync(int patientId)
        {
            return await _db.Referrals
                .Where(r => r.PatientId == patientId)
                .Include(r => r.Patient)
                .Include(r => r.Treatment)
                .Select(r => new ReferralResponse
                {
                    Id = r.Id,
                    PatientName = r.Patient.FirstName + " " + r.Patient.LastName,
                    TreatmentName = r.Treatment.Description,
                    CreatedAt = r.CreatedAt,
                    Status = "open"
            })
            .ToListAsync();
        }

        public async Task<IEnumerable<ReferralResponse>> GetDoctorReferralsAsync(int doctorId)
        {
            return await _db.Referrals
                .Where(r => r.DoctorId == doctorId)
                .Include(r => r.Patient)
                .Include(r => r.Treatment)
                .Select(r => new ReferralResponse
                {
                    Id = r.Id,
                    PatientName = r.Patient.FirstName + " " + r.Patient.LastName,
                    TreatmentName = r.Treatment.Description,
                    CreatedAt = r.CreatedAt,
                    Status = "open"
                })
                .ToListAsync();
        }
    }
}