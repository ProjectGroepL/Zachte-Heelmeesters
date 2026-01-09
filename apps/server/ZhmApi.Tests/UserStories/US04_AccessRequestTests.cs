using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZhmApi.Tests.Infrastructure;
using ZhmApi.Tests.POM;
using ZhmApi.Services;
using ZhmApi.Models;

[TestClass]
public class US04_AccessRequestTests : TestBase
{
    [TestMethod]
    public async Task Specialist_Vraagt_Toegang_Patient_Ontvangt_Melding()
    {
        var context = CreateContext();

        // Arrange
        context.Referrals.Add(new Referral { Id = 1, PatientId = 10 });
        context.Appointments.Add(new Appointment
        {
            Id = 1,
            SpecialistId = 5,
            ReferralId = 1
        });
        await context.SaveChangesAsync();

        var service = new AccessRequestService(context);
        var pom = new AccessRequestPom(service);

        // Act
        var request = await pom.RequestAccess(5, 1, "Voorbereiding");

        // Assert
        Assert.AreEqual(AccessRequestStatus.Pending, request.Status);
        Assert.AreEqual(10, request.PatientId);
        Assert.AreEqual(1, context.Notifications.Count());
    }
}
