using ZhmApi.Models;

namespace ZhmApi.Dtos
{
    public class ReferralDto
    {
        public int Id { get; set; }
        public string TreatmentDescription { get; set; } = null!;
        public string? Instructions { get; set; }
        public ReferralStatus Status { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAt {get; set;}
        public DateTime ValidUntil {get; set;}
    }
}
