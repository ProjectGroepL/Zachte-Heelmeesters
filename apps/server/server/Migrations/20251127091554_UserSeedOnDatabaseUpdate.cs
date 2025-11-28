using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ZhmApi.Migrations
{
    /// <inheritdoc />
    public partial class UserSeedOnDatabaseUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Regular patient with access to personal medical records.", "Patient" },
                    { 2, "Medical doctor responsible for patient care.", "General Practitioner" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "City", "Country", "Email", "FirstName", "GeneralPractitionerId", "HouseNumber", "HouseNumberAddition", "LastName", "MiddleName", "PasswordHash", "PhoneNumber", "PostalCode", "Province", "RoleId", "Street", "TwoFactorEnabled", "ZipCode" },
                values: new object[,]
                {
                    { 1, "Sampletown", "Country", "john.doe@example.com", "John", null, "12", null, "Doe", null, "hashedpassword123", "1234567890", "12345", "Province", 2, "Main Street", false, "12345" },
                    { 2, "Examplecity", "Country", "alice.smith@example.com", "Alice", 1, "34", null, "Smith", null, "hashedpassword456", "0987654321", "54321", "Province", 1, "Second Street", false, "54321" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
