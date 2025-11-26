using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZhmApi.Services;
using ZhmApi.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using ZhmApi.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace ZhmApi.Tests
{
    [TestClass]
    public class TwoFactorServiceTests
    {
        private ApiContext _db = null!;
        private TwoFactorService _twoFa = null!;
        private Mock<IEmailSender> _emailMock = null!;
        private IConfiguration _config = null!;
        
        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _db = new ApiContext(options);
            _emailMock = new Mock<IEmailSender>();
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["TWO_FACTOR_SECRET"] = "TEST_SECRET"
                })
                .Build();

            _twoFa = new TwoFactorService(_db, _emailMock.Object, _config);
        }
        private User CreateValidUser(int id, string email)
        {
            return new User
            {
                Id = id,
                Email = email,
                PasswordHash = "hash",
                FirstName = "Test",
                LastName = "User",
                PhoneNumber = "0612345678",
                Street = "Test Street",
                HouseNumber = "1",
                City = "TestCity",
                PostalCode = "1234AB",
                ZipCode = "1234AB",  // <-- toegevoegd
                Province = "TestProvince",
                Country = "NL",
                TwoFactorEnabled = true
            };
        }
        [TestMethod]
        public async Task Login_Triggers_2FA_And_Sends_Email()
        {
            // Arrange
            var user = CreateValidUser(1, "test@test.com");
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Act
            var sessionId = await _twoFa.CreateAndSendCodeAsync(user.Id, user.Email);

            // Assert
            Assert.IsTrue(sessionId > 0);
            _emailMock.Verify(x => x.SendAsync("test@test.com",
                It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task Verify_With_Wrong_Code_Fails()
        {
            var user = CreateValidUser(1, "test@test.com");
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var sessionId = await _twoFa.CreateAndSendCodeAsync(user.Id, user.Email);

            var result = await _twoFa.VerifyCodeAsync(sessionId, "999999");

            Assert.IsFalse(result.success);
            Assert.AreEqual("invalid_code", result.reason);
        }

        [TestMethod]
        public async Task Expired_Code_Should_Give_Error()
        {
            var user = CreateValidUser(1, "test@test.com");
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var sessionId = await _twoFa.CreateAndSendCodeAsync(user.Id, "test@test.com");

            var codeEntry = await _db.TwoFactorCodes.FindAsync(sessionId);
            codeEntry!.ExpiresAt = DateTime.UtcNow.AddMinutes(-1);
            await _db.SaveChangesAsync();

            var result = await _twoFa.VerifyCodeAsync(sessionId, "111111");

            Assert.IsFalse(result.success);
            Assert.AreEqual("expired", result.reason);
        }
    }
}
