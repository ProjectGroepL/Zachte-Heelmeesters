using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZhmApi.Tests.Infrastructure;
using ZhmApi.Tests.POM;
using ZhmApi.Services;
using ZhmApi.Models;
using Moq;

[TestClass]
public class RoleTest: TestBase
{
    [TestMethod]
    public async Task Admin_Assigns_Role_To_User()
    {
        var mock = new Mock<IRoleService>();
        mock.Setup(s => s.UserHasRoleAsync(2, "Admin"))
            .ReturnsAsync(true);

        var pom = new RoleAdminPom(mock.Object);

        Assert.IsTrue(await pom.GetUserRoles(2, "Admin"));
    }
}
