using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Models;
using ZhmApi.Services;
using ZhmApi.Dtos;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace ZhmApi.Tests
{
    [TestClass]
    public class ReferralServiceTests
    {
        private ApiContext _db = null!;
        private ReferralService _service = null!;
        private int _doctorId = 1;
        private int _patientId = 2;
        private int _treatmentId = 10;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _db = new ApiContext(options);
            _service = new ReferralService(_db);

            _db.Users.Add(new User
            {
                Id = _doctorId,
                Email = "doc@test.com",
                FirstName = "Doc",
                LastName = "Test",
                Street = "Street",
                HouseNumber = "1",
                City = "TestCity",
                ZipCode = "1234AB",
                Country = "NL"
            });

            _db.Users.Add(new User
            {
                Id = _patientId,
                Email = "patient@test.com",
                FirstName = "Patient",
                LastName = "Test",
                Street = "Street",
                HouseNumber = "2",
                City = "TestCity",
                ZipCode = "5678CD",
                Country = "NL"
            });
            _db.Treatments.Add(new Treatment { Id = _treatmentId, Description = "Test" });

            // Assign patient to doctor
            _db.DoctorPatients.Add(new DoctorPatients
            {
                DoctorId = _doctorId,
                PatientId = _patientId
            });

            _db.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup() => _db.Dispose();

        private CreateReferralRequest ValidRequest =>
            new CreateReferralRequest
            {
                PatientId = _patientId,
                TreatmentId = _treatmentId,
                Notes = "Test reason"
            };

        // ================================
        // 1️⃣ Successful Referral Creation
        // ================================
        [TestMethod]
        public async Task CreateReferral_Should_Succeed_When_Valid()
        {
            var result = await _service.CreateReferralAsync(_doctorId, ValidRequest);

            Assert.IsTrue(result.Succes);
            Assert.IsNull(result.Error);

            var saved = await _db.Referrals.FirstOrDefaultAsync();
            Assert.IsNotNull(saved);
            Assert.AreEqual(_doctorId, saved.DoctorId);
            Assert.AreEqual(_patientId, saved.PatientId);
        }

        // =======================================
        // 2️⃣ Patient Not Assigned To Doctor
        // =======================================
        [TestMethod]
        public async Task CreateReferral_Should_Fail_When_Patient_Not_Assigned()
        {
            var request = new CreateReferralRequest
            {
                PatientId = 999,
                TreatmentId = _treatmentId
            };

            var result = await _service.CreateReferralAsync(_doctorId, request);

            Assert.IsFalse(result.Succes);
            Assert.AreEqual("Patient niet toegestaan", result.Error);
        }

        // =================================
        // 3️⃣ Duplicate Referral Check
        // =================================
        [TestMethod]
        public async Task CreateReferral_Should_Fail_When_Duplicate()
        {
            await _service.CreateReferralAsync(_doctorId, ValidRequest);
            var result = await _service.CreateReferralAsync(_doctorId, ValidRequest);

            Assert.IsFalse(result.Succes);
            Assert.AreEqual("deze doorverwijzing bestaat al", result.Error);
            Assert.AreEqual(1, _db.Referrals.Count());
        }

        // ==================================
        // 4️⃣ Missing Required Fields
        // ==================================
        [TestMethod]
        public async Task CreateReferral_Should_Fail_When_Missing_Data()
        {
            var request = new CreateReferralRequest
            {
                PatientId = 0,
                TreatmentId = 0
            };

            var result = await _service.CreateReferralAsync(_doctorId, request);

            Assert.IsFalse(result.Succes);
            Assert.AreEqual("Patient niet toegestaan", result.Error);
        }
    }
}
