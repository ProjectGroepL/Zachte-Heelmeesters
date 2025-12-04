namespace ZhmApi.Dtos
{
    public class AppointmentDto
    {
        public int ReferralId { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TreatmentDescription { get; set; } = string.Empty;
        public string TreatmentInstructions { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
    }
}