namespace ZhmApi.Dtos
{
    public class ReferralDto
    {
        public int Id { get; set; }
        public string TreatmentDescription { get; set; } = null!;
        public string? Instructions { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
    }
}
