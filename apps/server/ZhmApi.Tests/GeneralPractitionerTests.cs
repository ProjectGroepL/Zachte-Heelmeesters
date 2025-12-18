using Microsoft.VisualStudio.TestTools.UnitTesting; // [TestClass], [TestMethod], Assert
using Microsoft.EntityFrameworkCore;               // DbContextOptionsBuilder, InMemoryDatabase
using Microsoft.AspNetCore.Mvc;                     // IActionResult, OkObjectResult
using ZhmApi.Data;                                  // ApiContext, User, UserRole
using ZhmApi.Controllers;                           // DoctorPatientsController
using ZhmApi.Models;                                    // User, UserRole
using ZhmApi.Dtos;                          
using System.Threading.Tasks;                       // Task, async/await
using System.Linq;                                  // eventueel voor ToList() en LINQ in asserts  
using Microsoft.AspNetCore.Identity;

namespace ZhmApi.Tests
{
    [TestClass]
    public class GeneralPractitionerTests
    {
        private ApiContext _db = null!;
        private DoctorPatientsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _db = new ApiContext(options);

            // Add test users
            var testUsers = new User[]
            {
                new User
                {
                    Id = 1,
                    FirstName = "Anna",
                    MiddleName = "van",
                    LastName = "Vliet",
                    ZipCode = "1000AA",
                    Street = "Main",
                    HouseNumber = "1",
                    City = "Amsterdam",
                    Country = "NL",
                    UserName = "admin@zhm.nl",
                    NormalizedUserName = "ADMIN@ZHM.NL",
                    Email = "admin@zhm.nl",
                    NormalizedEmail = "ADMIN@ZHM.NL",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAEOapgr1q7qX6qUoqajMQsj8QegZXQPGUTWwsBkbx2TBmidpq8A1r5DU4uGfxcw5kEQ==",
                    SecurityStamp = "URJYERSP245W5R6SNE2KEQ3ZHCLSRS2E",
                    ConcurrencyStamp = "e2715262-83be-4194-96c7-d2ad554e3dfc",
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                },

                new User
                {
                    Id = 2,
                    FirstName = "Test",
                    MiddleName = null,
                    LastName = "User",
                    ZipCode = "1000AA",
                    Street = "Main",
                    HouseNumber = "1",
                    City = "Amsterdam",
                    Country = "NL",
                    UserName = "admin@zhm.nl",
                    NormalizedUserName = "ADMIN@ZHM.NL",
                    Email = "admin@zhm.nl",
                    NormalizedEmail = "ADMIN@ZHM.NL",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAEOapgr1q7qX6qUoqajMQsj8QegZXQPGUTWwsBkbx2TBmidpq8A1r5DU4uGfxcw5kEQ==",
                    SecurityStamp = "URJYERSP245W5R6SNE2KEQ3ZHCLSRS2E",
                    ConcurrencyStamp = "e2715262-83be-4194-96c7-d2ad554e3dfc",
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                }

            };

            _db.Users.AddRange(testUsers);
            _db.UserRoles.AddRange(
                new IdentityUserRole<int> { UserId = 1, RoleId = 3 }, //doctor
                new IdentityUserRole<int> { UserId = 2, RoleId = 2 } // not doctor
            );

            _db.SaveChanges();
            _controller = new DoctorPatientsController(_db);
        }

        [TestMethod]
        public async Task GetDoctors_ReturnsOnlyGPs()
        {
            var result = await _controller.GetDoctors() as OkObjectResult;
            Assert.IsNotNull(result);

            var doctors = (result.Value as IEnumerable<object>)
                ?.Select(d => new DoctorDtoTest
                {
                    id = (int)d.GetType().GetProperty("id")!.GetValue(d),
                    fullName = (string)d.GetType().GetProperty("fullName")!.GetValue(d)!
                }).ToList();

            Assert.AreEqual(1, doctors.Count);
            Assert.AreEqual("Anna van Vliet", doctors[0].fullName.Trim());
        }

    }
}