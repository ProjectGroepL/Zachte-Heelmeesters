using System.ComponentModel.DataAnnotations;

namespace ZhmApi.Models
{
    public class SpecialistPrivateAgenda
    {
        public String Uid { get; set; } = null!;
        
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; } = null!; // specialist
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}