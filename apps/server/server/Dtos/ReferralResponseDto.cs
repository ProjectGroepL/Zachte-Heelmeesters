namespace ZhmApi.Dtos
{
    public class ReferralResponse
    {
        public int Id{get; set;}
        // Human-friendly names for the client
        public string PatientName { get; set; } = string.Empty;
        public string TreatmentName { get; set; } = string.Empty;
        public string Status { get; set; } = "open";
        public DateTime CreatedAt {get; set;}
    }
}