using System.ComponentModel.DataAnnotations;

namespace ZhmApi.Models
{
    public class SpecialistIcal
    {
        public int UserId { get; set; } // 1:1
        public User User { get; set; } = null!;
        
        public string Url { get; set; } = string.Empty; 
        public string? LastCalendarHash { get; set; } 
        public DateTime? LastSyncAt { get; set; }
    }
}