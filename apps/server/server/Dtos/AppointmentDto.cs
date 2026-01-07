using ZhmApi.Models;

namespace ZhmApi.Dtos
{
    public class AppointmentDto
    {
        public int Id { get; set; }  
        public int ReferralId { get; set; }
        public string Notes { get; set; } = string.Empty;
        public AppointmentStatus Status { get; set; }
        public string TreatmentDescription { get; set; } = string.Empty;
        public string TreatmentInstructions { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }
}