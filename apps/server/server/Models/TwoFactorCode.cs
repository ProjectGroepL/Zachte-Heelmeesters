using System.ComponentModel.DataAnnotations;

namespace ZhmApi.Models
{
    public class TwoFactorCode
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public string CodeHash { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Used { get; set; } = false;

        public int ResendCount { get; set; } = 0;
        public DateTime LastSentAt { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}
