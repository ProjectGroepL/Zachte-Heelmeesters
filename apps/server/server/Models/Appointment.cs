using System.ComponentModel.DataAnnotations.Schema;

namespace ZhmApi.Models
{
    public enum AppointmentStatus
    {
        PendingAccess,          // appointment exists, waiting on access
        Scheduled,              // access approved, operation allowed
        AccessDenied,           // access denied, appointment blocked
        Completed,
        Cancelled
    }
    public class Appointment
    {
        public int Id { get; set; }

        public int ReferralId { get; set; }
        [ForeignKey("ReferralId")]
        public Referral Referral { get; set; } = null!;
        public int SpecialistId {get; set;}
        public User Specialist {get; set;} = null!;
        public DateTime Date {get; set;}
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    }
}