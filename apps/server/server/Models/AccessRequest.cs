using System.ComponentModel.DataAnnotations.Schema;

namespace ZhmApi.Models
{
    public enum AccessRequestStatus
    {
        Pending,
        Approved,
        Denied,
        Revoked
    }
    public class AccessRequest
    {
        public int Id {get; set;}
        [ForeignKey(nameof(Specialist))]
        
        public int SpecialistId {get; set;} 
        public User Specialist {get; set;} = null!;
        [ForeignKey(nameof(Patient))]
        
        public int PatientId {get; set;} 
        public User Patient {get; set;} = null!;
        
        public int AppointmentId {get; set;}
        public Appointment Appointment {get; set;} = null!;
        
        public int TreatmentId {get; set;}
        public Treatment Treatment {get; set;} = null!; 
        public AccessRequestStatus Status {get; set;} 
        = AccessRequestStatus.Pending; 
        
        public string Reason {get; set;} = string.Empty;
        public DateTime RequestedAt {get; set;} = DateTime.UtcNow;
        public DateTime? DecidedAt {get; set;}
    }
}