using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZhmApi.Controllers;
using ZhmApi.Models;
using ZhmApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZhmApi.Tests.TestBase;

namespace ZhmApi.Tests.Referrals
{
    [TestClass]
    public class ReferralPatientEndpointTests : ControllerTestBase
    {
        private ReferralsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            SetupContextAndUser();

            var treatment = new Treatment
            {
                Id = 1,
                Description = "Test behandeling",
                Instructions = "Test instructies"
            };

            var referral = new Referral
            {
                PatientId = TestUser.Id,
                Treatment = treatment,
                Status = ReferralStatus.Open,
                CreatedAt = DateTime.UtcNow,
                ValidUntil = DateTime.UtcNow.AddDays(7)
            };

            Context.Treatments.Add(treatment);
            Context.Referrals.Add(referral);
            Context.SaveChanges();

            _controller = new ReferralsController(null!, Context);
            AttachUser(_controller);
        }

        [TestMethod]
        public async Task GetReferrals_Returns_Active_Referral()
        {
            var result = await _controller.GetReferrals();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);

            var referrals = ok.Value as List<ReferralDto>;
            Assert.IsNotNull(referrals);
            Assert.AreEqual(1, referrals.Count);
            Assert.AreEqual("Test behandeling", referrals[0].TreatmentDescription);
        }

        [TestMethod]
        public async Task GetReferrals_Expired_Referral_Is_Filtered()
        {
            Context.Referrals.First().ValidUntil = DateTime.UtcNow.AddDays(-1);
            Context.SaveChanges();

            var result = await _controller.GetReferrals();
            var ok = result.Result as OkObjectResult;

            var referrals = ok!.Value as List<ReferralDto>;
            Assert.AreEqual(0, referrals!.Count);
        }
    }
}
