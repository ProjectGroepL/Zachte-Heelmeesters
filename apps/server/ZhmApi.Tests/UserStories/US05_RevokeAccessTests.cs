using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZhmApi.Tests.Infrastructure;
using ZhmApi.Tests.POM;
using ZhmApi.Services;
using ZhmApi.Models;

[TestClass]
public class US05_RevokeAccessTests : TestBase
{
    [TestMethod]
    public async Task Patient_Trekt_Toestemming_In()
    {
        var context = CreateContext();
        var request = TestDataBuilder.CreateApprovedRequest(context);

        var service = new AccessRequestService(context);

        await service.RevokeAccess(request.Id, 10);

        Assert.AreEqual(AccessRequestStatus.Revoked, request.Status);
        Assert.AreEqual(AppointmentStatus.Cancelled, request.Appointment!.Status);
    }

    [TestMethod]
    public async Task Geen_Toestemming_Tonen_Foutmelding()
    {
        var context = CreateContext();
        var service = new AccessRequestService(context);

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            service.RevokeAccess(99, 10));
    }
}
