using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZhmApi.Tests.Infrastructure;
using ZhmApi.Tests.POM;
using ZhmApi.Services;
using ZhmApi.Models;

[TestClass]
public class InsuranceNotificationTests : TestBase
{
    public static class TestUserFactory
{
    public static User CreateValidUser(int id)
    {
        return new User
        {
            Id = id,
            FirstName = "Test",
            LastName = "User",
            Email = $"user{id}@test.nl",
            Street = "Teststraat",
            HouseNumber = "1",
            ZipCode = "1000AA",
            City = "Amsterdam",
            Country = "NL"
        };
    }
}
    [TestMethod]
    public async Task When_Invoice_Is_Sent_Insurer_Gets_Notification()
    {
        //Arange 
        var context = CreateContext();

        var insurer = TestUserFactory.CreateValidUser(100);
        context.Users.Add(insurer);
        context.SaveChanges();

        var notificationService = new NotificationService(context);
        var pom = new InsuranceNotificationPom(notificationService);

        // Act 
        await notificationService.Create(
            insurer.Id,
            "Nieuwe declaratie ontvangen: Factuur #123",
            NotificationType.General 
        );

        var notifications = await pom.GetNotifications(insurer.Id);

        // Assert
        //Assert.AreEqual(1, Notification.Count);
        Assert.IsFalse(notifications[0].Message.Contains("diagnose"));
        Assert.IsFalse(notifications[0].Message.Contains("behandeling"));
    }
    [TestMethod]
    public async Task Covered_Amount_Cannot_Exceed_Total_Amount()
    {
        var context = CreateContext();

        var invoice = new InsuranceInvoice
        {
            Id = 1,
            InsurerId = 10,
            Amount = 200
        };

        context.InsuranceInvoices.Add(invoice);
        context.SaveChanges();

        var service = new InsuranceInvoiceService(
            context,
            new NotificationService(context),
            null! // UserManager niet nodig
        );

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            service.SetCoveredAmount(1, 10, 300)
        );
    }


    [TestMethod]
    public async Task Covered_Amount_Cannot_Be_Negative()
    {
        var context = CreateContext();

        var invoice = new InsuranceInvoice
        {
            Id = 1,
            InsurerId = 10,
            Amount = 200
        };

        context.InsuranceInvoices.Add(invoice);
        context.SaveChanges();

        var service = new InsuranceInvoiceService(
            context,
            new NotificationService(context),
            null!
        );

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            service.SetCoveredAmount(1, 10, -50)
        );
    }
    [TestMethod]
    public async Task When_Invoice_Is_Covered_Patient_Gets_Notification()
    {
        var context = CreateContext();

        var patient = TestUserFactory.CreateValidUser(20);
        context.Users.Add(patient);
        context.SaveChanges();

        var invoice = new InsuranceInvoice
        {
            Id = 1,
            InsurerId = 10,
            Amount = 200,
            AppointmentReport = new AppointmentReport
            {
                Appointment = new Appointment
                {
                    Referral = new Referral
                    {
                        PatientId = patient.Id
                    }
                }
            }
        };

        context.InsuranceInvoices.Add(invoice);
        context.SaveChanges();

        var notificationService = new NotificationService(context);
        var service = new InsuranceInvoiceService(
            context,
            notificationService,
            null!
        );

        await service.SetCoveredAmount(1, 10, 150);

        var notifications =
            context.Notifications
                .Where(n => n.UserId == patient.Id)
                .ToList();

        Assert.AreEqual(1, notifications.Count);
    }
}