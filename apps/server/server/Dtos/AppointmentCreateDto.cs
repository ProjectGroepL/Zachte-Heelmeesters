namespace ZhmApi
{
    public class AppointmentCreateDto
    {
        public int ReferralId { get; set; }
        public string Date { get; set; } // YYYY-MM-DD
        public string Time { get; set; } // HH:MM
    }
}