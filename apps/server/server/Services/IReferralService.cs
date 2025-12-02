using ZhmApi.Dtos;
using ZhmApi.Models;

namespace ZhmApi.Services
{
    public interface IReferralService
    {
        Task<(bool Succes, String? Error, Referral? Referral)> CreateReferralAsync(int doctorId, CreateReferralRequest request);
        Task<Referral?> GetReferralAsync(int id);
        Task<IEnumerable<ReferralResponse>> GetPatientReferralAsync(int patientId);
    }
}