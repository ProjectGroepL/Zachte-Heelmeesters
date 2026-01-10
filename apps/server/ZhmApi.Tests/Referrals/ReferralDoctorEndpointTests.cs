using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ZhmApi.Controllers;
using ZhmApi.Services;
using ZhmApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZhmApi.Tests.TestBase;

namespace ZhmApi.Tests.Referrals
{
    [TestClass]
    public class ReferralDoctorEndpointTests : ControllerTestBase
    {
        private Mock<IReferralService> _serviceMock = null!;
        private ReferralsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            SetupContextAndUser();

            _serviceMock = new Mock<IReferralService>();
            _controller = new ReferralsController(_serviceMock.Object, Context);

            AttachUser(_controller);
        }

        [TestMethod]
        public async Task GetForCurrentDoctor_Returns_Data_From_Service()
        {
            _serviceMock
                .Setup(s => s.GetDoctorReferralsAsync(TestUser.Id))
                .ReturnsAsync(new List<ReferralResponse>
                {
                    new ReferralResponse
                    {
                        Id = 1,
                        PatientName = "John Doe",
                        TreatmentName = "Behandeling",
                        Status = "Open"
                    }
                });

            var result = await _controller.GetForCurrentDoctor();
            var ok = result as OkObjectResult;

            Assert.IsNotNull(ok);
        }
    }
}
