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

    [TestCleanup]
    public void Cleanup()
    {
      _db.Dispose();
    }

    private User CreateValidUser(int id, string email)
    {
      return new User
      {
        Id = id,
        Email = email,
        FirstName = "Test",
        MiddleName = null,
        LastName = "User",
        PhoneNumber = "0612345678",
        Street = "Test Street",
        HouseNumber = "1",
        City = "TestCity",
        ZipCode = "1234AB",
        Country = "NL",
        TwoFactorEnabled = true,
        RoleId = 1
      };
    }

    [TestMethod]
    public async Task CreateAndSendCodeAsync_Should_Create_Code_And_Send_Email()
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

      var codeEntry = await _db.TwoFactorCodes.FindAsync(sessionId);
      Assert.IsNotNull(codeEntry);
      Assert.AreEqual(user.Id, codeEntry.UserId);
      Assert.IsFalse(codeEntry.Used);
      Assert.AreEqual(0, codeEntry.ResendCount);
    }

    [TestMethod]
    public async Task VerifyCodeAsync_With_Wrong_Code_Should_Fail()
    {
      // Arrange
      var user = CreateValidUser(1, "test@test.com");
      _db.Users.Add(user);
      await _db.SaveChangesAsync();

      var sessionId = await _twoFa.CreateAndSendCodeAsync(user.Id, user.Email);

      // Act
      var result = await _twoFa.VerifyCodeAsync(sessionId, "999999");

      // Assert
      Assert.IsFalse(result.success);
      Assert.AreEqual("invalid_code", result.reason);
    }

    [TestMethod]
    public async Task VerifyCodeAsync_With_Expired_Code_Should_Fail()
    {
      // Arrange
      var user = CreateValidUser(1, "test@test.com");
      _db.Users.Add(user);
      await _db.SaveChangesAsync();

      var sessionId = await _twoFa.CreateAndSendCodeAsync(user.Id, "test@test.com");

      // Expire the code
      var codeEntry = await _db.TwoFactorCodes.FindAsync(sessionId);
      codeEntry!.ExpiresAt = DateTime.UtcNow.AddMinutes(-1);
      await _db.SaveChangesAsync();

      // Act
      var result = await _twoFa.VerifyCodeAsync(sessionId, "111111");

      // Assert
      Assert.IsFalse(result.success);
      Assert.AreEqual("expired", result.reason);
    }

    [TestMethod]
    public async Task VerifyCodeAsync_With_Already_Used_Code_Should_Fail()
    {
      // Arrange
      var user = CreateValidUser(1, "test@test.com");
      _db.Users.Add(user);
      await _db.SaveChangesAsync();

      var sessionId = await _twoFa.CreateAndSendCodeAsync(user.Id, "test@test.com");

      // Mark code as used
      var codeEntry = await _db.TwoFactorCodes.FindAsync(sessionId);
      codeEntry!.Used = true;
      await _db.SaveChangesAsync();

      // Act
      var result = await _twoFa.VerifyCodeAsync(sessionId, "111111");

      // Assert
      Assert.IsFalse(result.success);
      Assert.AreEqual("already_used", result.reason);
    }

    [TestMethod]
    public async Task ResendAsync_Should_Increment_Resend_Count()
    {
      // Arrange
      var user = CreateValidUser(1, "test@test.com");
      _db.Users.Add(user);
      await _db.SaveChangesAsync();

      var sessionId = await _twoFa.CreateAndSendCodeAsync(user.Id, user.Email);

      // Act
      var result = await _twoFa.ResendAsync(sessionId);

      // Assert
      Assert.IsTrue(result.success);
      _emailMock.Verify(x => x.SendAsync(user.Email,
          It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2)); // Original + resend

      var codeEntry = await _db.TwoFactorCodes.FindAsync(sessionId);
      Assert.AreEqual(1, codeEntry!.ResendCount);
    }

    [TestMethod]
    public async Task ResendAsync_Should_Fail_After_Max_Resends()
    {
      // Arrange
      var user = CreateValidUser(1, "test@test.com");
      _db.Users.Add(user);
      await _db.SaveChangesAsync();

      var sessionId = await _twoFa.CreateAndSendCodeAsync(user.Id, user.Email);

      // Set resend count to max
      var codeEntry = await _db.TwoFactorCodes.FindAsync(sessionId);
      codeEntry!.ResendCount = 3;
      await _db.SaveChangesAsync();

      // Act
      var result = await _twoFa.ResendAsync(sessionId);

      // Assert
      Assert.IsFalse(result.success);
      Assert.AreEqual("too_many_resends", result.reason);
    }

    [TestMethod]
    public async Task ResendAsync_With_Invalid_Session_Should_Fail()
    {
      // Act
      var result = await _twoFa.ResendAsync(99999);

      // Assert
      Assert.IsFalse(result.success);
      Assert.AreEqual("no_session", result.reason);
    }
  }
}