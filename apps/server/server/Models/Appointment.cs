using System.ComponentModel.DataAnnotations.Schema;

namespace ZhmApi.Models
{
    public enum AppointmentStatus
    {
        Scheduled,
        CancelledAccessDenied,
        CancelledByPatient,
        CancelledBySpecialist,
        Completed
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