using System.ComponentModel.DataAnnotations.Schema;

namespace ZhmApi.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public int ReferralId { get; set; }
        [ForeignKey("ReferralId")]
        public Referral Referral { get; set; }

        public int TreatmentId { get; set; }
        [ForeignKey("TreatmentId")]
        public Treatment Treatment { get; set; }
    }
}