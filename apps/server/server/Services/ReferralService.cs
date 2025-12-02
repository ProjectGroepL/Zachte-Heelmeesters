using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Dtos;
using ZhmApi.Models;

namespace ZhmApi.Services
{
    public class ReferralService : IReferralService
    {
        private readonly ApiContext _db;

        public async Task<(bool Succes, string? Error, Referral? Referral)> CreateReferralAsync(
            int doctorId, CreateReferralRequest request)
        {
            bool allowed = await _db.DoctorPatients
                .AnyAsync(dp => dp.DoctorId == doctorId && dp.PatientId == request.patientId);
            if(!allowed)
                return(false, "Patient niet toegestaan", null);

            bool exists = await _db.Referrals
                .AnyAsync(r => r.PatientId == request.patientId && r.TreatmentId == request.TreatmentId);

            if(exists)
                return(false, "deze doorverwijzing bestaat al", null);   

            var referral = new Referral
            {
                DoctorId = doctorId,
                PatientId = request.patientId,
                TreatmentId = request.TreatmentId 
            };

            _db.Referrals.Add(referral);
            await _db.SaveChangesAsync();

            return (true,null,referral);
        }
        public async Task<Referral?> GetReferralAsync(int id)
        {
            return await _db.Referrals
                    .Include(r => r.Treatment)
                    .FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<IEnumerable<ReferralResponse>>  GetPatientReferralAsync(int patientId)
        {
            return await _db.Referrals
                .Where(r => r.PatientId == patientId)
                .Select(r => new ReferralResponse
                {
                    Id = r.Id,
                    Treatment = r.Treatment.Description,
                    CreatedAt = r.CreatedAt 
            })
            .ToListAsync();
        }
    }
}