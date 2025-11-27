using System.ComponentModel.DataAnnotations;

namespace ZhmApi.Dtos
{
  public class RegisterDto
  {
    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Street is required")]
    public string Street { get; set; } = null!;

    [Required(ErrorMessage = "House number is required")]
    public string HouseNumber { get; set; } = null!;

    public string? HouseNumberAddition { get; set; }

    [Required(ErrorMessage = "Zip code is required")]
    public string ZipCode { get; set; } = null!;

    [Required(ErrorMessage = "City is required")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "Country is required")]
    public string Country { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; } = null!;
  }
}