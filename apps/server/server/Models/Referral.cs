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

        public DateTime CreatedAt {get; set; }= DateTime.UtcNow;


    }
}