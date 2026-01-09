using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZhmApi.Tests.Infrastructure;
using ZhmApi.Tests.POM;
using ZhmApi.Services;
using ZhmApi.Models;
[TestClass]
public class US38_GrantAccessTests : TestBase
{
    [TestMethod]
    public async Task Patient_Verleent_Toegang_Aan_Specialist()
    {
        var context = CreateContext();
        var request = TestDataBuilder.CreatePendingRequest(context);

        var service = new AccessRequestService(context);

        await service.DecideRequest(request.Id, 10, true);

        Assert.AreEqual(AccessRequestStatus.Approved, request.Status);
    }
}
