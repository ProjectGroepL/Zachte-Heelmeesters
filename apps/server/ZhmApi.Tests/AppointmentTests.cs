using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Controllers;
using ZhmApi.Data;
using ZhmApi.Models;
using ZhmApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace ZhmApi.Tests
{
    [TestClass]
    public class AppointmentsControllerTests
    {
        private ApiContext _context = null!;
        private AppointmentsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;


            _context = new ApiContext(options);

            // Add user
            var testUser = new User
            {
                Id = 1,
                FirstName = "System",
                MiddleName = null,
                LastName = "Admin",
                ZipCode = "1000AA",
                Street = "Main",
                HouseNumber = "1",
                HouseNumberAddition = null,
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
                PhoneNumber = null,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = true,
                AccessFailedCount = 0
            };
            _context.Users.Add(testUser);

            // Add referral
            var referral = new Referral
            {
                Id = 1,
                PatientId = testUser.Id,
                Status = "open",
                CreatedAt = DateTime.UtcNow,
                Treatment = new Treatment
                {
                    Id = 1,
                    Description = "Test behandeling",
                    Instructions = "Test instructies"
                }
            };
            _context.Referrals.Add(referral);

            _context.SaveChanges();

            _controller = new AppointmentsController(_context);

            // Mock user identity
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, testUser.Id.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [TestMethod]
        public async Task CreateAppointment_ValidReferral_ReturnsOk()
        {
            var dto = new AppointmentCreateDto
            {
                ReferralId = 1,
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Time = "10:00"
            };

            var result = await _controller.CreateAppointment(dto);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task CreateAppointment_InvalidReferral_ReturnsBadRequest()
        {
            var dto = new AppointmentCreateDto
            {
                ReferralId = 999, // Niet bestaande referral
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Time = "10:00"
            };

            var result = await _controller.CreateAppointment(dto);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}
