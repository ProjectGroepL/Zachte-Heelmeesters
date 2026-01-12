using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZhmApi.Tests.Infrastructure;
using ZhmApi.Tests.POM;
using ZhmApi.Services;
using ZhmApi.Models;
[TestClass]
public class US20_AccessLifecycleTests : TestBase
{
    [TestMethod]
    public async Task Goedgekeurde_Toegang_Geeft_Inzage_En_Logt()
    {
        var context = CreateContext();
        var request = TestDataBuilder.CreatePendingRequest(context);

        var service = new AccessRequestService(context);

        await service.DecideRequest(request.Id, 10, true);

        Assert.AreEqual(AccessRequestStatus.Approved, request.Status);
        Assert.AreEqual(1, context.Notifications.Count());
    }

    [TestMethod]
    public async Task Geweigerde_Toegang_Wordt_Gelogd()
    {
        var context = CreateContext();
        var request = TestDataBuilder.CreatePendingRequest(context);

        var service = new AccessRequestService(context);

        await service.DecideRequest(request.Id, 10, false);

        Assert.AreEqual(AccessRequestStatus.Denied, request.Status);
    }
}
