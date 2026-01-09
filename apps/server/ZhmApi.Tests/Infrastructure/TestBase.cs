using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;

namespace ZhmApi.Tests.Infrastructure
{
    public abstract class TestBase
    {
        protected ApiContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApiContext(options);
        }
    }
}
