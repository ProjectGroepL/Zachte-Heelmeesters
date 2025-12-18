using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Controllers;
using ZhmApi.Data;
using ZhmApi.Models;
using ZhmApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using ZhmApi.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace ZhmApi.Tests
{
    [TestClass]
    public class ReferralTests
    {
        private ApiContext _context = null!;
        private ReferralsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            // In-memory database setup
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApiContext(options);

            // Add test user
            var testUser = new User
            {
                Id = 1,
                FirstName = "System",
                MiddleName = null,
                LastName = "Admin",
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
            };
            _context.Users.Add(testUser);

            // Add referral
            var referral = new Referral
            {
                Id = 1,
                PatientId = testUser.Id,
                Status = "open",
                CreatedAt = DateTime.UtcNow,
                ValidUntil = DateTime.UtcNow.AddDays(7),
                Treatment = new Treatment
                {
                    Id = 1,
                    Description = "Test behandeling",
                    Instructions = "Test instructies"
                }
            };
            _context.Referrals.Add(referral);
            _context.SaveChanges();

            // Fake service voor controller
            var fakeService = new FakeReferralService();

            // Controller instellen
            _controller = new ReferralsController(fakeService, _context);

            // Mock user identity
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, testUser.Id.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TestMethod]
        public async Task GetReferrals_ValidReferral_ReturnsReferral()
        {
            var result = await _controller.GetReferrals();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);

            var referrals = ok?.Value as List<ReferralDto>;
            Assert.IsNotNull(referrals);
            Assert.AreEqual(1, referrals.Count);
            Assert.AreEqual("Test behandeling", referrals[0].TreatmentDescription);
        }

        [TestMethod]
        public async Task GetReferrals_NoReferrals_ReturnsEmptyList()
        {
            _context.Referrals.RemoveRange(_context.Referrals);
            _context.SaveChanges();

            var result = await _controller.GetReferrals();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);

            var referrals = ok?.Value as List<ReferralDto>;
            Assert.IsNotNull(referrals);
            Assert.AreEqual(0, referrals.Count);
        }

        [TestMethod]
        public async Task GetReferrals_ExpiredReferral_IsFilteredOut()
        {
            var referral = await _context.Referrals.FirstAsync();
            referral.ValidUntil = DateTime.UtcNow.AddDays(-1);
            _context.SaveChanges();

            var result = await _controller.GetReferrals();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);

            var referrals = ok?.Value as List<ReferralDto>;
            Assert.IsNotNull(referrals);
            Assert.AreEqual(0, referrals.Count);
        }
    }

    // Fake service voor de controller, exact dezelfde interface als IReferralService
    public class FakeReferralService : IReferralService
    {
        public Task<(bool Succes, string? Error, Referral? Referral)> CreateReferralAsync(int doctorId, CreateReferralRequest request)
            => Task.FromResult<(bool, string?, Referral?)>((true, null, null));

        public Task<Referral?> GetReferralAsync(int id)
            => Task.FromResult<Referral?>(null);

        public Task<IEnumerable<ReferralResponse>> GetPatientReferralAsync(int patientId)
            => Task.FromResult<IEnumerable<ReferralResponse>>(new List<ReferralResponse>());

        public Task<IEnumerable<ReferralResponse>> GetDoctorReferralsAsync(int doctorId)
            => Task.FromResult<IEnumerable<ReferralResponse>>(new List<ReferralResponse>());
    }

}
