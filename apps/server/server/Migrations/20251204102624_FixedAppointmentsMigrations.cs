using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZhmApi.Migrations
{
    /// <inheritdoc />
    public partial class FixedAppointmentsMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "153d0c2b-500c-40a8-991b-2557e8242566");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "ff77d632-3585-49b7-9566-32c9222ef748");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "eb413989-b648-4380-aed3-d137fb8733b0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "3c2544fa-802c-4d19-95bb-4ec74a8c82cf");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "61694bd1-9775-4d35-b898-ba32bf2dc769");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                column: "ConcurrencyStamp",
                value: "38b682fd-ccbf-4551-bbc7-03bb6cd1cde3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "988819c4-8ee0-45c5-b18c-2448c7e2aaae");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "a41bdaa4-c189-4c83-b017-e250c53f968d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "620d814f-c800-44da-9b6c-b7221d6f2c7b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "f0ec3e50-5a3d-4743-8dc5-f557135e16d3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "66c619ef-cda6-41f5-a63a-8a8292cfe1bd");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                column: "ConcurrencyStamp",
                value: "c7ad8cd4-2db1-4a75-ae5b-7401d2fe2a59");
        }
    }
}
