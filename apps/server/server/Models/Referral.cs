namespace ZhmApi.Models 
{
    public class Referral
    {
        public int Id {get; set;}
        public int PatientId {get; set;}
        public User Patient {get; set;} = null!;
        
        public int DoctorId {get; set;}
        public User Doctor {get; set;} = null!;

        public int TreatmentId {get; set;}
        public Treatment Treatment {get; set;} = null!;

        // Optional free-form notes supplied by the referring doctor
        public string? Notes { get; set; }

        // Current referral status (e.g. open, accepted, closed)
        public string Status { get; set; } = "open";

        public DateTime CreatedAt {get; set; } = DateTime.UtcNow;
        public DateTime ValidUntil {get; set;}

        public Referral()
        {
            CreatedAt = DateTime.UtcNow;
            ValidUntil = CreatedAt.AddMonths(1);
        }


    }
}