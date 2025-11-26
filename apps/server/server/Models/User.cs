namespace ZhmApi.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string ZipCode { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public bool TwoFactorEnabled { get; set; } = false;

        public string Street { get; set; } = null!;

        public string HouseNumber { get; set; } = null!;

        public string? HouseNumberAddition { get; set; }

        public string PostalCode { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Province { get; set; } = null!;

        public string Country { get; set; } = null!;

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}