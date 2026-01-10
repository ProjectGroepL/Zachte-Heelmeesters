using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZhmApi.Data;
using ZhmApi.Models;

namespace ZhmApi.Tests.TestBase
{
    public abstract class ControllerTestBase
    {
        protected ApiContext Context = null!;
        protected User TestUser = null!;

        protected void SetupContextAndUser(int userId = 1)
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            Context = new ApiContext(options);

            TestUser = new User
            {
                Id = userId,
                FirstName = "System",
                LastName = "Admin",
                Email = "admin@zhm.nl",
                City = "Amsterdam",
                ZipCode = "1000AA",
                Street = "Main",
                HouseNumber = "1",
                Country = "NL"
            };

            Context.Users.Add(TestUser);
            Context.SaveChanges();
        }

        protected void AttachUser(ControllerBase controller)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new[] { new Claim(ClaimTypes.NameIdentifier, TestUser.Id.ToString()) },
                            "mock"
                        )
                    )
                }
            };
        }
    }
}
