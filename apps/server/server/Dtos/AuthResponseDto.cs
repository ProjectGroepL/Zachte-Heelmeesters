namespace ZhmApi.Dtos
{
  public class AuthResponseDto
  {
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public UserDto User { get; set; } = null!;
  }

  public class RefreshTokenDto
  {
    public string RefreshToken { get; set; } = null!;
  }

  public class UserDto
  {
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Street { get; set; } = null!;
    public string HouseNumber { get; set; } = null!;
    public string? HouseNumberAddition { get; set; }
    public string ZipCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public bool TwoFactorEnabled { get; set; }
    public string Role { get; set; } = null!;
  }
}