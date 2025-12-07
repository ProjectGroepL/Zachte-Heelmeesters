namespace ZhmApi.Models{
public class DoctorPatients
{
    public int DoctorId { get; set; }
    public User? Doctor { get; set; }

    public int PatientId { get; set; }
    public User? Patient { get; set; }
}
}