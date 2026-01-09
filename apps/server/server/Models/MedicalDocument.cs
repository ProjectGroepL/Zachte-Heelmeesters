using System.ComponentModel.DataAnnotations.Schema;

namespace ZhmApi.Models
{
    public enum MedicalDocumentStatus
    {
        Draft = 0,
        Final = 1,
        Archived = 2
    }
    public class MedicalDocument{
        public int Id {get; set;}
        [ForeignKey(nameof(Patient))]

        public int PatientId {get; set;}
        public User Patient {get; set;} = null!;

        public int? TreatmentId {get; set;}
        public Treatment? Treatment {get; set;} = null!;

        public int? AppointmentId {get; set;}
        public Appointment? Appointment {get; set;} = null!;
        
        public string Title {get; set;} = null!;
        public string Content {get; set;} = string.Empty;
        
        public DateTime createdAt {get; set;} = DateTime.UtcNow;
        public string CreatedBy {get; set;} = null!;
        
        public MedicalDocumentStatus Status {get; set;} 
        = MedicalDocumentStatus.Draft; 

    }
    
}