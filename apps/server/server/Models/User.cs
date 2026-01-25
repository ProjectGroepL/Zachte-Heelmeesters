using Microsoft.AspNetCore.Identity;

namespace ZhmApi.Models
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = null!;

        public string ZipCode { get; set; } = null!;

        public string Street { get; set; } = null!;

        public string HouseNumber { get; set; } = null!;

        public string? HouseNumberAddition { get; set; }

        public string City { get; set; } = null!;

        public string Country { get; set; } = null!;

        public ICollection<DoctorPatients> DoctorPatients { get; set; } = new List<DoctorPatients>();
    }
}