using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using ZhmApi.Controllers;
using ZhmApi.Data;
using ZhmApi.Models;
using ZhmApi.Dtos;

namespace ZhmApi.Tests
{
    [TestClass]
    public class PatientsControllerTests
    {
        private ApiContext GetInMemoryDbContext([CallerMemberName] string testName = "")
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{testName}_{Guid.NewGuid()}")
                .Options;

            var context = new ApiContext(options);

            // Seed roles
            var patientRole = new Role { Id = 1, Name = "Patient", Description = "Patient role" };
            var gpRole = new Role { Id = 2, Name = "GP", Description = "General Practitioner role" };
            context.Roles.AddRange(patientRole, gpRole);

            // Seed users
            context.Users.AddRange(
                new User
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    PhoneNumber = "1234567890",
                    ZipCode = "1234AB",
                    PasswordHash = "dummyhash",
                    Street = "Main St",
                    HouseNumber = "1A",
                    PostalCode = "1234AB",
                    City = "CityX",
                    Province = "ProvinceX",
                    Country = "CountryX",
                    RoleId = 1,
                    Role = patientRole
                },
                new User
                {
                    Id = 2,
                    FirstName = "Alice",
                    LastName = "Smith",
                    Email = "alice@example.com",
                    PhoneNumber = "0987654321",
                    ZipCode = "5678CD",
                    PasswordHash = "dummyhash",
                    Street = "Second St",
                    HouseNumber = "2B",
                    PostalCode = "5678CD",
                    City = "CityY",
                    Province = "ProvinceY",
                    Country = "CountryY",
                    RoleId = 2,
                    Role = gpRole
                },
                new User
                {
                    Id = 3,
                    FirstName = "Bob",
                    MiddleName = "A.",
                    LastName = "Brown",
                    Email = "bob@example.com",
                    PhoneNumber = "1122334455",
                    ZipCode = "9012EF",
                    PasswordHash = "dummyhash",
                    Street = "Third St",
                    HouseNumber = "3C",
                    PostalCode = "9012EF",
                    City = "CityZ",
                    Province = "ProvinceZ",
                    Country = "CountryZ",
                    RoleId = 2,
                    Role = gpRole
                }
            );

            context.SaveChanges();
            return context;
        }


        [TestMethod]
        public async Task GetGps_ReturnsAllGPs_WhenNoSearch()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new PatientsController(context);

            // Act
            var result = await controller.GetGps(null);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            Assert.IsNotNull(okResult.Value);
            var gps = (IEnumerable<GpDto>)okResult.Value;

            Assert.IsNotNull(gps);
            Assert.AreEqual(2, gps.Count());
        }

        [TestMethod]
        public async Task GetGps_ReturnsFilteredGPs_WhenSearchProvided()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new PatientsController(context);

            // Act
            var result = await controller.GetGps("Bob");

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            Assert.IsNotNull(okResult.Value);
            var gps = (IEnumerable<GpDto>)okResult.Value;


            var gpList = gps.ToList();
            Assert.AreEqual(1, gpList.Count);
            Assert.AreEqual("Bob A. Brown", gpList[0].FullName);
        }

        [TestMethod]
        public async Task SetGeneralPractitioner_AssignsGpSuccessfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new PatientsController(context);

            var dto = new SetGpDto { GeneralPractitionerId = 2 };

            // Act
            var result = await controller.SetGeneralPractitioner(1, dto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var patient = context.Users.Find(1);
            Assert.AreEqual(2, patient.GeneralPractitionerId);
        }

        [TestMethod]
        public async Task SetGeneralPractitioner_ReturnsNotFound_WhenPatientNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new PatientsController(context);

            var dto = new SetGpDto { GeneralPractitionerId = 2 };

            // Act
            var result = await controller.SetGeneralPractitioner(999, dto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Patient not found", notFoundResult.Value);
        }

        [TestMethod]
        public async Task SetGeneralPractitioner_ReturnsNotFound_WhenGpNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new PatientsController(context);

            var dto = new SetGpDto { GeneralPractitionerId = 999 };

            // Act
            var result = await controller.SetGeneralPractitioner(1, dto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("General Practitioner not found", notFoundResult.Value);
        }
    }
}
