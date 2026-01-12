namespace ZhmApi
{
    public class AppointmentCreateDto
    {
        public int ReferralId { get; set; }
        public int SpecialistId { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }
}