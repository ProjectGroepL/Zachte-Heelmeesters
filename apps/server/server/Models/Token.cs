using System.ComponentModel.DataAnnotations;

namespace ZhmApi.Models
{
  public enum TokenType
  {
    RefreshToken
  }

  public class Token
  {
    public int Id { get; set; }

    [Required]
    public string Value { get; set; } = null!;

    [Required]
    public TokenType Type { get; set; }

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; } = false;

    // Computed property to check if token is valid
    public bool IsValid => !IsRevoked && ExpiresAt > DateTime.UtcNow;
  }
}